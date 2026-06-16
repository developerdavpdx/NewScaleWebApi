    using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.LineasServ;
using WebApiPtmHana.BLL.Services.SkuInfoServ;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.Reportes;
using static System.Net.WebRequestMethods;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SapPtmController : ControllerBase
    {
        private readonly HanaConnection _hanaconnection;
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly ISkuInfo _skuinfoService;
        private readonly string prefijo;
        private readonly string connectionStringSQL;
        public SapPtmController(HanaConnection hanaConnection, IConfiguration configuration, ISkuInfo skuInfo) 
        { 
            _hanaconnection = hanaConnection;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            _skuinfoService = skuInfo;
            connectionStringSQL = configuration.GetConnectionString("SQLConnection");
            //Llamamos al prefijo para los stores
            prefijo = configuration["StoredProcedurePrefix"];
        }

        /// <summary>
        /// Obtenemos todos los sku
        /// </summary>
        /// <param name="planta"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllPT")]
        public async Task<IActionResult> GetAllOwor([FromHeader] string planta)
        {
            try
            {
                //obtenemos los sku y sus datos dependiendo de la planta
                IList<SkuInfo> skuInfos = await _skuinfoService.GetAllSkuInfos(int.Parse(planta));

                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using(HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetSkuPT";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        using(HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    var JsonOwor = JsonConvert.DeserializeObject<List<InfoOwor>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JsonOwor, skuInfo = skuInfos }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { ex.Message});
            }
        }

        [HttpGet]
        [Route("GetPTbySku")]
        public IActionResult GetSkuDetail([FromHeader] string sku, [FromHeader] string planta)
        {
            try
            {
                IList<SkuInfo> skuInfos = _skuinfoService.GetBySku(sku, planta);
                //return StatusCode(StatusCodes.Status200OK, new {  skuInfos }); // Devolvemos el resultado en JSON
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetDetailSKU";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("SKU", HanaDbType.NVarChar).Value = sku;
                        using (HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    var JsonOwor = JsonConvert.DeserializeObject<List<InfoOwor>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JsonOwor, skuInfos }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetFabricOrder")]
        public IActionResult GetFabricOrder([FromHeader] string series)
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using(HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetProductionOrder";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("Series", HanaDbType.NVarChar).Value = series;
                        using (HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    //var JsonOwor = JsonConvert.DeserializeObject<List<InfoOrderFabric>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JSONString }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetFabricOrderDetail")]
        public IActionResult GetFabricOrderDetail([FromHeader] string docentry)
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using(HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetDetailFO";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("DocEntry", HanaDbType.NVarChar).Value = docentry;
                        using (HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    var JsonOwor = JsonConvert.DeserializeObject<List<InfoOrderFabric>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JsonOwor }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetNewScaleItems")]
        public IActionResult GetNewScaleItems()
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPDDX_GetUNewScale";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        using (HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    //var JsonOwor = JsonConvert.DeserializeObject<List<InfoOrderFabric>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JSONString }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetItemName")]
        public IActionResult GetItemName([FromHeader] string itemCode)
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GETITEMNAME";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("ItemCode", HanaDbType.NVarChar).Value = itemCode;
                        using (HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    var JsonOwor = JsonConvert.DeserializeObject<List<ItemCode>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JsonOwor }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("UpdateNwScaleItem")]
        public IActionResult UpdateNwScaleItem([FromHeader] string itemcode)
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection conn = new HanaConnection(connectionString))
                {
                    if(conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using(HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_UPDATENWSCALEITEM";
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("ITEMCODE", HanaDbType.NVarChar).Value = itemcode;
                        using(HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    var JsonOwor = JsonConvert.DeserializeObject<List<NwScaleItem>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { data = JsonOwor }); // Devolvemos el resultado en JSON
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }

                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }


        /// <summary>
        /// obtenemos la info por linea parael sku seleccionado
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="planta"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerInfoLinea")]
        public IActionResult ObtenerInfoLinea([FromHeader] string sku, [FromHeader] string planta)
        {
            try
            {
                string JSONstring = string.Empty;

                using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                {
                    if (cnn.State == ConnectionState.Closed)
                    {
                        cnn.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = cnn;
                        command.CommandText = "Sppdx_ObtenerKgHoraPorLinea";
                        command.CommandType = CommandType.StoredProcedure;
                        //parametros del procedure
                        command.Parameters.AddWithValue("@sku", sku);
                        command.Parameters.AddWithValue("@planta", planta);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tablaInfo = dtDocuments.Tables[0];

                            using (tablaInfo)
                            {
                                JSONstring = JsonConvert.SerializeObject(tablaInfo);
                                var SkuDetailLinea = JsonConvert.DeserializeObject<List<SkuDetailLinea>>(JSONstring);
                                
                                return StatusCode(StatusCodes.Status200OK, new { SkuDetailLinea });
                            }
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { ex.Message });
            }
        }

        /// <summary>
        /// Actualiza la info por lineas de los sku
        /// </summary>
        /// <param name="lineasData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ActualizarInfoLineas")]
        public IActionResult UpdateDataLinea([FromBody] List<LineaData> lineasData)
        {
            try
            {
                string JSONstring = string.Empty;

                using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                {
                    if (cnn.State == ConnectionState.Closed)
                    {
                        cnn.Open();
                    }

                    foreach (var linea in lineasData)
                    {
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = cnn;
                            command.CommandText = "Sppdx_ActualizarSkuInfo";
                            command.CommandType = CommandType.StoredProcedure;

                            // Parámetros del procedure
                            command.Parameters.AddWithValue("@id", linea.Id);
                            command.Parameters.AddWithValue("@sku", linea.Sku);
                            command.Parameters.AddWithValue("@linea", linea.Linea);
                            command.Parameters.AddWithValue("@planta", linea.Planta);
                            command.Parameters.AddWithValue("@KgHora", linea.KgHora);                            

                            command.ExecuteNonQuery();
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Datos actualizados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { ex.Message });
            }
        }

        /// <summary>
        /// añadimos datos nuevos de linea (codigo, kgHora, linea)
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="planta"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AddNewDataLine")]
        public IActionResult AddNewDataLine([FromHeader] string kgHora, [FromHeader] string linea,
            [FromHeader] string planta, [FromHeader] string codigo)
        {
            try
            {
                // Convertimos las variables de string al tipo esperado por el procedure
                if (!decimal.TryParse(kgHora, out decimal kgHoraDecimal))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Formato inválido para kgHora" });
                }

                // Aseguramos que el valor decimal tenga los decimales esperados
                kgHoraDecimal = decimal.Parse(kgHoraDecimal.ToString("0.0000"));

                if (!int.TryParse(linea, out int lineaInt))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Formato inválido para linea" });
                }

                if (!int.TryParse(planta, out int plantaInt))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Formato inválido para planta" });
                }


                using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                {
                    if (cnn.State == ConnectionState.Closed)
                    {
                        cnn.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = cnn;
                        command.CommandText = "Sppdx_AgregarKgHoraxLinea";
                        command.CommandType = CommandType.StoredProcedure;
                        //parametros del procedure
                        command.Parameters.AddWithValue("@kgHora", kgHoraDecimal);
                        command.Parameters.AddWithValue("@linea", lineaInt);
                        command.Parameters.AddWithValue("@planta", plantaInt);
                        command.Parameters.AddWithValue("@codigo", codigo);

                        command.ExecuteNonQuery();
                    }
                    return StatusCode(StatusCodes.Status200OK, new { message = "Datos añadidos correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
        }
    }
}
