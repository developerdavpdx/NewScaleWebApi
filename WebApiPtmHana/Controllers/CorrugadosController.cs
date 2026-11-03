using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Sap.Data.Hana;
using System.Data;
using System.Text.Json;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.DatosCorrugados;

[Route("api/[controller]")]
[ApiController]
public class CorrugadosController : ControllerBase
{
    private readonly string connectionStringHANA;
    private readonly string connectionStringSQL;
    private readonly string prefijo;

    public CorrugadosController(IConfiguration configuration)
    {
        connectionStringSQL = configuration.GetConnectionString("SQLConnection");
        connectionStringHANA = configuration.GetConnectionString("DefaulConnection");
        //Llamamos al prefijo para los stores (bd de SAP)
        prefijo = configuration["StoredProcedurePrefix"];
    }

    //-------WEB--------//
    //-------DATOS MAESTROS--------//
    [HttpGet]
    [Route("GetDataMaster")]    //Obtiene todos los datos de los articulos por planta
    public async Task<IActionResult> GetDataMaster([FromHeader] string planta)
    {
        try
        {
            var ArticulosData = new List<object>();

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "Sppdx_CorrugadosGeneral";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 10); //obtiene todos los datos de los articulos existentes en la tabla
                    command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Json", "");
                    command.Parameters.AddWithValue("@Linea", "");
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    //ejecutamos y leemos el sp
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            ArticulosData.Add(row);
                        }
                    }
                }
            }
            //evaluamos que existan registros
            if (ArticulosData.Count == 0)
            {
                return NotFound(new { status = 404, message = "No se encontraron artículos" });
            }

            return Ok(new { status = 200, data = ArticulosData });

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, response = ex.Message });
        }
    }

    //agrega (marca) en SAP el codigo que sera usado para funcionar con nwscale (U_U_PDX_NWSCALE = 1)
    [HttpPost]
    [Route("AddArticuloToNwScale")]
    public IActionResult AddArticuloToNwScale([FromBody] JsonElement data)
    {
        try
        {
            string jsonParam = data.GetRawText();

            using (HanaConnection connection = new HanaConnection(connectionStringHANA))
            {
                connection.Open();

                using (HanaCommand command = new HanaCommand())
                {
                    command.CommandText = $"{prefijo}.SpPdxNWScale_CorrugadosGeneral_SAP";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("OPC", HanaDbType.Integer).Value = 3;
                    command.Parameters.Add("Json", HanaDbType.NVarChar).Value = jsonParam;

                    using (HanaDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int status = int.Parse(reader["STATUS"].ToString());
                            string message = reader["MESSAGE"].ToString();

                            if (status == 200)
                            {
                                CreateArticuloBD(data);
                            }

                            // 🔥 SIEMPRE HTTP 200
                            return Ok(new { status, message });
                        }
                        else
                        {
                            return Ok(new
                            {
                                status = 500,
                                message = "Sin respuesta del SP"
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                status = 500,
                message = "Error servidor: " + ex.Message
            });
        }
    }

    // Método privado para agregar el codigo marcado, a la db de sql [en tabla skuInfos] (se agrega solo con su proceso respectivo para que DatosCorrugado.js lo pueda encontrar de acuerdo al proceso, [aun se debe ejecutar la actualizacion despues de agregarlo])
    private void CreateArticuloBD(JsonElement data)
    {
        var jsonData = JsonDocument.Parse(data.GetRawText());
        var root = jsonData.RootElement;

        // valores por default
        string code = string.Empty;
        int planta = 0;
        string proceso = string.Empty;
        string nameUser = string.Empty;

        // code
        if (root.TryGetProperty("code", out JsonElement codeElement))
        {
            code = codeElement.GetString() ?? string.Empty;
        }

        // planta
        if (root.TryGetProperty("planta", out JsonElement plantaElement))
        {
            if (plantaElement.TryGetInt32(out int plantaValue))
            {
                planta = plantaValue;
            }
        }

        // proceso
        if (root.TryGetProperty("proceso", out JsonElement procesoElement))
        {
            proceso = procesoElement.GetString() ?? string.Empty;
        }

        // usuario
        if (root.TryGetProperty("NameUser", out JsonElement userElement))
        {
            nameUser = userElement.GetString() ?? string.Empty;
        }

        // Creamos el objeto JSON que será pasado al SP
        var datos = new Dictionary<string, object>
            {
                { "code", code },
                { "NameUser", nameUser }
            };

        string jsonParamSQL = JsonConvert.SerializeObject(datos);   //convertimos a json

        using (SqlConnection connection = new SqlConnection(connectionStringSQL))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@OPC", 11); // Agrega una nueva línea
                command.Parameters.AddWithValue("@Proceso", (proceso != string.Empty ? proceso : "Corrugado"));   //El proceso siempre es "corrugados" (pues se agrega desde el menu de "corrugados")
                command.Parameters.AddWithValue("@Json", jsonParamSQL);
                //parametros que no se usan, pero forman parte del SP
                command.Parameters.AddWithValue("@Planta", planta);
                command.Parameters.AddWithValue("@Linea", "");

                command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                command.ExecuteNonQuery();
            }
        }
    }

    //-------ASIGNACION DE LINEAS--------//
    [HttpGet]
    [Route("GetLiniesCorrugados")]  //Obtenemos las Lineas existentes de corrugados
    public IActionResult GetLiniesCorrugados([FromHeader] string planta)
    {
        try
        {
            var lineas = new List<object>();

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "Sppdx_CorrugadosGeneral";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 0); //obtiene lineas existentes
                    command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Linea", "");
                    command.Parameters.AddWithValue("@Json", "");
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lineas.Add(new
                            {
                                numeroLineas = reader["NumeroLineas"],
                                ordenFabricacion = reader["OrdenFabricacion"],
                                kgXHr = reader["kgXHr"],
                                itemCodeLinea = reader["itemCodeLinea"],
                                asignado = reader["asignado"],
                                numLinea = reader["numLinea"],
                                proceso = reader["proceso"]
                            });
                        }
                    }
                }
            }
            return Ok(new { status = 200, response = lineas });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, response = ex.Message });
        }
    }


    [HttpPost]
    [Route("CreateNewLinieCorrugados")]     //Crea una nueva linea de corrugados
    public IActionResult CreateNewLinieCorrugados([FromHeader] string idLinea, [FromHeader] string proceso, [FromHeader] string planta)
    {
        try
        {
            int result;
            string errorMessage = "";
            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 1); //Agrega una nueva linea
                    command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                    command.Parameters.AddWithValue("@Linea", int.Parse(idLinea));
                    command.Parameters.AddWithValue("@Proceso", proceso);
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Json", "");
                    //Parametros de resultado y error
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int){ Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000){ Direction = ParameterDirection.Output });

                    command.ExecuteNonQuery();

                    //leemos el valor de los parametros de salida (@Result y @ErrorMessage)
                    result = (int)command.Parameters["@Result"].Value;
                    errorMessage = command.Parameters["@ErrorMessage"].Value?.ToString() ?? "";

                }
            }

            if (result == 1)
            {
                return StatusCode(StatusCodes.Status200OK, new { response = "Se ha creado la línea correctamente" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Error al crear la línea: " + errorMessage });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, response = ex.Message });
        }
    }

    [HttpPost]
    [Route("DeleteLinieCorrugados")]     //Borra una linea de corrugados
    public IActionResult DeleteLinieCorrugados([FromHeader] string idLinea, [FromHeader] string planta)
    {
        try
        {
            int result;
            string errorMessage = "";
            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 2); //Elimina una linea
                    command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                    command.Parameters.AddWithValue("@Linea", int.Parse(idLinea));
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Json", "");
                    //Parametros de resultado y error
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    command.ExecuteNonQuery();

                    //leemos el valor de los parametros de salida (@Result y @ErrorMessage)
                    result = (int)command.Parameters["@Result"].Value;
                    errorMessage = command.Parameters["@ErrorMessage"].Value?.ToString() ?? "";

                }
            }

            if (result == 1)
            {
                return StatusCode(StatusCodes.Status200OK, new { response = "Se ha eliminado la línea correctamente" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Error al eliminar la línea: " + errorMessage });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, response = ex.Message });
        }
    }

    [HttpGet]
    [Route("GetLiniesHistoryAsignedCorrugados")]  //Obtenemos el historial de asignaciones de la linea seleccionada de corrugados
    public IActionResult GetLiniesHistoryAsignedCorrugados([FromHeader] string linea, [FromHeader] string planta)
    {
        try
        {
            var AsignacionesHistorial = new List<object>();

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = "Sppdx_CorrugadosGeneral";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 3); //obtiene el historial de la linea seleccionada
                    command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                    command.Parameters.AddWithValue("@Linea", int.Parse(linea));
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Json", "");
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = reader["Usuario"] == DBNull.Value ? null : reader["Usuario"].ToString();
                            var fecha = reader["Fecha"] == DBNull.Value ? null : reader["Fecha"].ToString();
                            var codigo = reader["Codigo"] == DBNull.Value ? null : reader["Codigo"].ToString();

                            System.Diagnostics.Debug.WriteLine($"Usuario: {usuario}, Fecha: {fecha}, Codigo: {codigo}");

                            AsignacionesHistorial.Add(new
                            {
                                name = reader["Usuario"].ToString(),
                                dateasign = reader["Fecha"].ToString(),
                                codigo = reader["Codigo"].ToString(),
                            });
                        }
                    }
                }
            }
            return Ok(new { status = 200, data = AsignacionesHistorial });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, response = ex.Message });
        }
    }

    //Obtiene los articulos existentes desde SAP (para llenar las sugerencias)
    [HttpGet]
    [Route("GetArticulosSAP")]
    public IActionResult GetArticulosSAP()
    {
        try
        {
            HanaResponses hana = new HanaResponses();
            using (HanaConnection connection = new HanaConnection(connectionStringHANA))
            {
                connection.Open();
                using (HanaCommand command = new HanaCommand())
                {
                    command.CommandText = $"{prefijo}.SpPdxNWScale_CorrugadosGeneral_SAP";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.Add("OPC", HanaDbType.Integer).Value = 0;

                    // parametros que usa el sp, pero no se usan en este OPC (mandamos vacio)
                    command.Parameters.Add("Json", HanaDbType.NVarChar).Value = "{}";

                    using (HanaDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            hana.Items = new DataTable();
                            hana.Items.Load(reader);
                            using (hana.Items)
                            {
                                hana.response = new Dictionary<string, string>();
                                string Result = JsonConvert.SerializeObject(hana.Items);
                                var JsonResult = JsonConvert.DeserializeObject<List<InfoOwor>>(Result);

                                return Ok(new { status = 200, data = JsonResult });
                            }
                        }
                        else
                        {
                            hana.response = new Dictionary<string, string>();
                            return NotFound(new { status = 404, message = "No se encontraron artículos" });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al obtener los articulos de SAP: " + ex.Message });
        }
    }

    //Obtenemos Los kilogramos hora de la linea a la que seran asignados, de acuerdo al articulo elejido (solo si se le ha asignado un kgh)
    [HttpPost]
    [Route("GetKgHoraCorrugados")]
    public IActionResult GetKgHoraCorrugados([FromBody] dynamic data)
    {
        try
        {
            //obtenemos los datos enviados desde el js
            string code = data.GetProperty("code").GetString();
            int planta = data.GetProperty("planta").GetInt32();
            int linea = data.GetProperty("linea").GetInt32();

            // Creamos el objeto JSON que será pasado al SP
            var datos = new Dictionary<string, object>
            { 
                { "code", code }
            };
            string jsonCode = JsonConvert.SerializeObject(datos);   //convertimos a json

            object result;

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 5);
                    command.Parameters.AddWithValue("@Planta", planta);
                    command.Parameters.AddWithValue("@Linea", linea);
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    command.Parameters.AddWithValue("@Json", jsonCode);
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    // Ejecutamos y obtenemos el resultado (puede ser null)
                    result = command.ExecuteScalar();
                }
            }

            // Si no hay resultado, devolvemos null o vacío (no se encontro un kgh asignado para la linea y codigo elejidos)
            string kgHora = result == null || result == DBNull.Value ? "" : result.ToString();

            return Ok(new { status = 200, data = kgHora });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al obtener los kilogramos hora: " + ex.Message });
        }
    }

    //obtiene Las Ordenes de Fabricacion en base al articulo y planta
    [HttpPost("GetOF_SAP")]
    public IActionResult GetOF_SAP([FromBody] JsonElement data)
    {
        try
        {
            string jsonParam = data.GetRawText(); // Obtiene el JSON completo (enviado desde el front) como string para pasar al SP

            HanaResponses hana = new HanaResponses();
            using (HanaConnection connection = new HanaConnection(connectionStringHANA))
            {
                connection.Open();
                using (HanaCommand command = new HanaCommand())
                {
                    command.CommandText = $"{prefijo}.SpPdxNWScale_CorrugadosGeneral_SAP";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("OPC", HanaDbType.Integer).Value = 1;
                    command.Parameters.Add("Json", HanaDbType.NVarChar).Value = jsonParam;

                    using (HanaDataReader reader = command.ExecuteReader())
                    {
                        //si se encuentran OF
                        if (reader.HasRows)
                        {
                            hana.Items = new DataTable();
                            hana.Items.Load(reader);

                            string Result = JsonConvert.SerializeObject(hana.Items);
                            return Ok(new { status = 200, data = Result });
                        }
                        //si no hay OF
                        else
                        {
                            return NotFound(new { status = 404, message = "No se encontraron OF para el código seleccionado." });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al obtener las Ordenes de Fabricación: " + ex.Message });
        }
    }

    //Asigna una Linea (OF, codigo, etc..)
    [HttpPost("AsignarLinea")]
    public IActionResult AsignarLinea([FromBody] DataAsignarLinea data)
    {
        try
        {
            var datos = new Dictionary<string, object>
        {
            {"code", data.code},
            {"turno", data.turno},
            {"kgxhora", data.kgxhora},
            {"ordenfabricacion", data.ordenfabricacion},
            {"folio", data.folio},
            {"NameUser", data.NameUser}
        };
            string jsonCode = JsonConvert.SerializeObject(datos);

            int result;
            string errorMessage = "";

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 6); //asigna una linea
                    command.Parameters.AddWithValue("@Planta", data.planta);
                    command.Parameters.AddWithValue("@Linea", data.linea);
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    command.Parameters.AddWithValue("@Json", jsonCode);

                    //Parametros de resultado y error
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    command.ExecuteNonQuery();

                    //leemos el valor de los parametros de salida (@Result y @ErrorMessage)
                    result = (int)command.Parameters["@Result"].Value;
                    errorMessage = command.Parameters["@ErrorMessage"].Value?.ToString() ?? "";
                }
            }

            if (result == 1)
            {
                return Ok(new { status = 200, response = "Se ha asignado la línea correctamente." });
            }
            else
            {
                return NotFound(new { status = 404, message = $"Error al asignar la línea: {errorMessage}" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al asignar la Línea: " + ex.Message });
        }
    }

    //Asigna una Linea (OF, codigo, etc..)
    [HttpPost("Des_AsignarLinea")]
    public IActionResult Des_AsignarLinea([FromBody] DataGeneralLineaPlanta data)
    {
        try
        {
            int result;
            string errorMessage = "";

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 7); //DES-ASIGNA una linea
                    command.Parameters.AddWithValue("@Planta", data.planta);
                    command.Parameters.AddWithValue("@Linea", data.linea);
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");

                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Json", "");

                    //Parametros de resultado y error
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    command.ExecuteNonQuery();

                    //leemos el valor de los parametros de salida (@Result y @ErrorMessage)
                    result = (int)command.Parameters["@Result"].Value;
                    errorMessage = command.Parameters["@ErrorMessage"].Value?.ToString() ?? "";
                }
            }

            if (result == 1)
            {
                return Ok(new { status = 200, response = "Se ha Des-asignado la línea correctamente." });
            }
            else
            {
                return NotFound(new { status = 404, message = $"Error al Des-asignar la línea: {errorMessage}" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al Des-asignar la Línea: " + ex.Message });
        }
    }


    //-------OPERADOR--------//
    //-------PesajePT--------//
    [HttpGet]
    [Route("ValidarCodigoExiste")]  //Valida si el codigo existe en alguna de las lineas que tienen una asignacion
    public IActionResult ValidarCodigoExiste([FromHeader] string Linea, [FromHeader] string code, [FromHeader] string planta)
    {
        try
        {
            // Creamos el objeto con las variables que se enviarán en formato JSON
            var datos = new Dictionary<string, object>
                {
                    { "code", code }    // añadimos las variaables necesarias
                    //{ "linea", Linea },
                    //{ "planta", planta }
                };
            string jsonCode = JsonConvert.SerializeObject(datos); // generamos el json

            int existe = 0; //inicalizamos como que no se encontro el codigo (0 = no existe, 1 = existe)

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OPC", 4); // Verifica el código
                    command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                    command.Parameters.AddWithValue("@Linea", int.Parse(Linea));
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    command.Parameters.AddWithValue("@Json", jsonCode);
                    //parametros que no se usan, pero forman parte del SP
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    // Ejecutamos el SP esperando solo un valor (1 o 0)
                    object result = command.ExecuteScalar();

                    // Validamos el resultado, por seguridad (en caso de ser un DBNull)
                    if (result != DBNull.Value && result != null)
                        existe = Convert.ToInt32(result);   //asignamos el valor obtenido (0 o 1)
                }
            }
            //validamos para enviar el mensaje correcto
            bool existeCodigo;
            string mensaje;

            if (existe == 1)
            {
                existeCodigo = true;
                mensaje = "El código ya está asignado en la línea.";
            }
            else
            {
                existeCodigo = false;
                mensaje = "El código no está asignado en ninguna línea.";
            }

            return Ok(new { status = 200, existeCodigo, mensaje });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al validar si el codigo esta asignado: <br>" + ex.Message });
        }
    }


    [HttpGet]
    [Route("GetPesosPTM")]  //Calculamos los pesos e informacion relevante de acuerdo al codigo escaneado (para poder guardar el registro completo en bd)
    public IActionResult GetPesosPTM([FromHeader] double peso, [FromHeader] string code, [FromHeader] int numTubos, [FromHeader] int planta, [FromHeader] string? proceso)
    {
        try
        {
            //construimos el objeto para el json
            var DataSQL = new Dictionary<string, object>
            {
                { "code", code }
            };
            string jsonSQL = JsonConvert.SerializeObject(DataSQL);  //generamos el json

            //definimos la clase para guardar el resultado de la consulta
            DataPesosObtenidos data = null;

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@OPC", 8);
                    command.Parameters.AddWithValue("@Planta", planta);
                    command.Parameters.AddWithValue("@Proceso", proceso);
                    command.Parameters.AddWithValue("@Json", jsonSQL);
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Linea", "");
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    //guardamos los datos en DataPesosObtenidos para usarlos posteriormente en las formulas
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = new DataPesosObtenidos
                            {
                                NumTubos = reader["NumTubos"] != DBNull.Value ? Convert.ToInt32(reader["NumTubos"]) : 0,
                                Variable = reader["Variable"] != DBNull.Value ? Convert.ToDouble(reader["Variable"]) : 0,
                                AnilloCantidad = reader["AnilloCantidad"] != DBNull.Value ? Convert.ToDouble(reader["AnilloCantidad"]) : 0,
                                FlejeCantidad = reader["FlejeCantidad"] != DBNull.Value ? Convert.ToDouble(reader["FlejeCantidad"]) : 0,
                                ArpillaCantidad = reader["ArpillaCantidad"] != DBNull.Value ? Convert.ToDouble(reader["ArpillaCantidad"]) : 0,
                                TarimaCantidad = reader["TarimaCantidad"] != DBNull.Value ? Convert.ToDouble(reader["TarimaCantidad"]) : 0,
                                MaderaCantidad = reader["MaderaCantidad"] != DBNull.Value ? Convert.ToDouble(reader["MaderaCantidad"]) : 0,
                                CostalesCantidad = reader["CostalesCantidad"] != DBNull.Value ? Convert.ToDouble(reader["CostalesCantidad"]) : 0,
                                PesoMinimo = reader["PesoMinimo"] != DBNull.Value ? Convert.ToDouble(reader["PesoMinimo"]) : 0
                            };
                        }
                    }
                }
            }

            //si no existen datos para el codigo, significa que no han actualizado/agregado el codigo al sistema, unicamente en SAP
            if (data == null)
            {
                return NotFound(new { status = 404, message = "No se encontraron datos de Embalaje para el código escaneado.<br> Verifica que el codigo cuente con datos de embalaje" });
            }

            // **** -->> REALIZAMOS LAS OPERACIONES NECESARIAS [AQUI SE APLICAN LAS FORMULAS PROPORCIONADAS POR PTM] <<-- ****

            //verificamos si el usuario mando un atado incompleto
            int TubosTotales = 0;
            if (numTubos == 0)  //si viene en 0, significa que el usuario NO mando atado incompleto, por lo que dejamos el valor que viene de la consulta
            {
                TubosTotales = data.NumTubos;
            }
            else
            {
                TubosTotales = numTubos;    //el usuario mando atado incompleto, respetamos los tubos que envio
            }
            //Peso Total
            double PesoTotal = peso;
            //Peso Neto (el peso total menos las taras asignadas)
            double PesoNeto = PesoTotal - (data.Variable + data.AnilloCantidad + data.FlejeCantidad + data.ArpillaCantidad + data.TarimaCantidad + data.MaderaCantidad + data.CostalesCantidad);
            //Peso estandar unitario (numero te tubos por el peso minimo asignado)
            double PesoEstandarUnitario = TubosTotales * data.PesoMinimo;
            //Peso Estandar
            double PesoEstandar = TubosTotales * PesoEstandarUnitario;
            //Peso de cada tubo
            double PesoTubos = PesoNeto / TubosTotales;
            //SOBREPESO
            double SobrePeso = ((PesoNeto / PesoEstandarUnitario) - 1) * 100;
            //determinamos si el sobrepeso es alto, por debajo o esta ok (de acuerdo al proceso) para encender el semaforo y lanzar el modal de alert
            SobrepesoAlert SobrepesoDeterminar = new SobrepesoAlert();
            int AlertSobrepeso = SobrepesoDeterminar.SobrepesoAlerta(SobrePeso, proceso);

            return Ok(new
            {
                status = 200,
                data = new
                {
                    PesoNeto = PesoNeto,
                    SobrePeso = SobrePeso,
                    PesoTubos = PesoTubos,
                    AlertSobrepeso = AlertSobrepeso,
                    TubosTotales = TubosTotales,
                    PesoTotal = PesoTotal,
                    //datos que envio pero actualmente no se consumen despues de llegar al front, los reportes, graficas, etc, recalculan estos datos de manera global
                    PesoEstandarUnitario = PesoEstandarUnitario,
                    PesoEstandar = PesoEstandar
                },
                message = "Datos obtenidos correctamente",
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al calcular los pesos en GetPesosPTM: " + ex.Message });
        }
    }

    //para crear un nuevo registro de escaneo en BD (tablas TicketsInfo, ProductoTerminados e HistorialPesadas)
    [HttpPost("AddNewDBRegister")]
    public IActionResult AddNewDBRegister([FromBody] DataAddTicket data)
    {
        try
        {
            int Folio = 0;

            //------    HANA OBTENEMOS EL FOLIO DE ACUERDO AL CODIGO ESCANEADO
            var datosJsonHana = new Dictionary<string, object>
                {
                    { "code", data.code }
                };
            string jsonHana = JsonConvert.SerializeObject(datosJsonHana); // generamos el json

            HanaResponses hana = new HanaResponses();
            using (HanaConnection connection = new HanaConnection(connectionStringHANA))
            {
                connection.Open();
                using (HanaCommand command = new HanaCommand())
                {
                    command.CommandText = $"{prefijo}.SpPdxNWScale_CorrugadosGeneral_SAP";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("OPC", HanaDbType.Integer).Value = 2;    //obtiene el folio de acuerdo al codigo escaneado
                    command.Parameters.Add("Json", HanaDbType.NVarChar).Value = jsonHana;

                    //leemos el folio obtenido
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Folio = Convert.ToInt32(reader["FOLIO"]);
                        }
                        else
                        {
                            //si algo falla en el sp, lanzamos la exceocion para hacer debug
                            throw new Exception("Ocurrió un problema al obtener el folio en el SP de Hana [OPC = 2]");
                        }
                    }
                }
            }


            //------    SQL TERMINSMOS DE CREAR EL TICKET
            // Creamos el objeto con las variables que se enviarán en formato JSON
            var datosSQL = new Dictionary<string, object>
            {
                { "code", data.code },
                { "folio", Folio },
                { "turno", data.turno },

                { "calidad", data.calidad },
                { "pesoNeto", data.pesoNeto },
                { "sobrePeso", data.sobrePeso },
                { "pesotubos", data.pesotubos },
                { "numTubos", data.numTubos },
                { "pesoBascula", data.pesoBascula }
            };
            string jsonSQL = JsonConvert.SerializeObject(datosSQL); // generamos el json

            int ticketcreado = 0; //inicalizamos como que no se ha creado el ticket (0 = no se creo, 1 = si se creo)

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OPC", 9); // Crea un nuevo ticket y seleciona los datos de este ticket crado para completar el zpl para su impresion
                    command.Parameters.AddWithValue("@Planta", data.planta);
                    command.Parameters.AddWithValue("@Linea", data.linea);
                    command.Parameters.AddWithValue("@Proceso", "Corrugado");
                    command.Parameters.AddWithValue("@Json", jsonSQL);

                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    // Ejecutamos el SP y leemos el conjunto de resultados devuelto
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Leemos los valores del resultado (se creo o no el registro)
                            int result = reader.GetInt32(reader.GetOrdinal("Result"));
                            string errorMessage = reader.GetString(reader.GetOrdinal("ErrorMessage"));
                            //datos para generar el zpl e imprimir
                            string folio = reader.IsDBNull(reader.GetOrdinal("Folio")) ? null : reader.GetString(reader.GetOrdinal("Folio"));
                            string ordenFabricacion = reader.IsDBNull(reader.GetOrdinal("OrdenFabricacion")) ? null : reader.GetString(reader.GetOrdinal("OrdenFabricacion"));
                            string itemName = reader.IsDBNull(reader.GetOrdinal("ItemName")) ? null : reader.GetString(reader.GetOrdinal("ItemName"));
                            int operacion = reader.IsDBNull(reader.GetOrdinal("Operacion")) ? 0 : reader.GetInt32(reader.GetOrdinal("Operacion"));

                            //validamos si el registro fue creado o no
                            bool ticketCreated = (result == 1);
                            string mensaje = ticketCreated ? "El ticket se creó correctamente." : $"Error: {errorMessage}";

                            return Ok(new
                            {
                                status = 200,
                                TicketCreated = ticketCreated,
                                mensaje = mensaje,
                                Folio = folio,
                                OrdenFabricacion = ordenFabricacion,
                                ItemName = itemName,
                                Operacion = operacion
                            });
                        }
                        else
                        {
                            // No se recibieron datos del SP, enviamos error
                            return StatusCode(500, new { status = 500, message = "No se recibieron datos del stored procedure" });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al intentar crear un ticket en controller AddNewTicket: <br>" + ex.Message });
        }
    }


    //-------Envio Scrap--------//
    //Obtiene el nombre/descripcion del codigo escaneado (si existe el codigo en sap)
    [HttpPost]
    [Route("GetDescriptionCodeScrap")]
    public IActionResult GetDescriptionCodeScrap([FromBody] JsonElement data)
    {
        try
        {
            string jsonParam = data.GetRawText(); // Obtiene el JSON completo (enviado desde el front) como string para pasar al SP
            string code = data.GetProperty("code").GetString(); //obtenemos el codigo como dato puro para retornarlo en el mensaje en caso de requerirse

            HanaResponses hana = new HanaResponses();
            using (HanaConnection connection = new HanaConnection(connectionStringHANA))
            {
                connection.Open();
                using (HanaCommand command = new HanaCommand())
                {
                    command.CommandText = $"{prefijo}.SpPdxNWScale_CorrugadosGeneral_SAP";
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros
                    command.Parameters.Add("OPC", HanaDbType.Integer).Value = 4;
                    command.Parameters.Add("Json", HanaDbType.NVarChar).Value = jsonParam;

                    //leemos el resultado y verificamos que el codigo escaneado haya tenido una coincidencia en SAP
                    using (HanaDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var result = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                            return Ok(new { status = 200, data = result });
                        }
                        else
                        {
                            return NotFound(new { status = 404, message = $"El código escaneado, <strong>{code}</strong> no se encontró en SAP" });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al obtener los artículos de SAP: " + ex.Message });
        }
    }

    //obtiene el id mas reciente de los registros de scrap (para sumarle 1 y mostrar el preview en el front)
    [HttpGet("GetScrapID")]
    public IActionResult GetScrapID()
    {
        try
        {
            int IdScrapEncontrado = 0;

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OPC", 12); // Obtiene el id mas reciente de scraps
                    //parametros que no se usan, pero forman parte del SP
                    command.Parameters.AddWithValue("@Planta", "");
                    command.Parameters.AddWithValue("@Linea", "");
                    command.Parameters.AddWithValue("@Proceso", "");
                    command.Parameters.AddWithValue("@Json", "");
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    // Ejecutamos el SP
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int scrapId))
                    {
                        IdScrapEncontrado = scrapId;
                    }
                }
            }
            return Ok(new { status = 200, IdScrapEncontrado });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al intentar guardar el scrap, en controller GetScrapID: <br>" + ex.Message });
        }
    }

    //Para crear un registro de SCRAP en BD
    [HttpPost("AddNewDBRegisterScrap")]
    public IActionResult AddNewDBRegisterScrap([FromBody] DataAddScrap data)
    {
        try
        {
            // Creamos el objeto con las variables que se enviarán en formato JSON
            var datosSQL = new Dictionary<string, object>
            {
                { "nameUser", data.nameUser },
                { "code", data.code },
                { "pesoBascula", data.pesoBascula },
                { "turno", data.turno },
                { "coments", data.coments },
                { "familia", data.familia },
                { "scrapCorteSierra", data.scrapCorteSierra }
            };
            string jsonSQL = JsonConvert.SerializeObject(datosSQL); // generamos el json

            int scrapCreado = 0; //inicalizamos como que no se ha creado el ticket (0 = no se creo, 1 = si se creo)

            using (SqlConnection connection = new SqlConnection(connectionStringSQL))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sppdx_CorrugadosGeneral", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@OPC", 13); // Crea un nuevo registro de scrap
                    command.Parameters.AddWithValue("@Planta", data.planta);
                    command.Parameters.AddWithValue("@Linea", data.linea);
                    command.Parameters.AddWithValue("@Proceso", data.proceso);
                    command.Parameters.AddWithValue("@Json", jsonSQL);
                    //parametros que no se usan, pero forman parte del SP
                    //Parametros de resultado y error (no se usan en esta OPC)
                    command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output });

                    // Ejecutamos el SP esperando solo un valor (1 o 0)
                    command.ExecuteNonQuery();
                    scrapCreado = (int)command.Parameters["@Result"].Value;
                }
            }
            //validamos para enviar el mensaje correcto
            bool ScrapCreated;
            string mensaje;

            if (scrapCreado == 1)
            {
                ScrapCreated = true;
                mensaje = "El scrap se envio correctamente.";
            }
            else
            {
                ScrapCreated = false;
                mensaje = "Ocurrio un error al guardar el scrap en BD, en el SP de sql [OPC 13] en controller 'AddNewDBRegisterScrap'. Favor de reintentar";
            }

            return Ok(new { status = 200, ScrapCreated, mensaje });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al intentar guardar el scrap, en controller AddNewDBRegisterScrap: <br>" + ex.Message });
        }
    }

    //nextcontroller
}
