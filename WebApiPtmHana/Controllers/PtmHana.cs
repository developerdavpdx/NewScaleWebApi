using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Renci.SshNet;
using Sap.Data.Hana;
using WebApiPtmHana.Models;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PtmHana : ControllerBase
    {
        private readonly HanaConnection hanaConnection;
        private readonly string connectionString;
        private readonly string prefijo;

        public PtmHana(HanaConnection _hanaConnection, IConfiguration configuration)
        {
            hanaConnection = _hanaConnection;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            //Llamamos al prefijo para los stores
            prefijo = configuration["StoredProcedurePrefix"];
        }



        [HttpGet]
        [Route("GetLinesProduction")]
        public IActionResult GetLinesProduction()
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    //Abre la conexión a SAP Hana
                    connection.Open();
                    // Se crea la instancia para el ejecutar comandos a Hana
                    using (HanaCommand command = new HanaCommand())
                    {
                        // Creamos el comando a ejecutar en Sap Hana (DBName.NombreStoreProcedure)
                        //Checar el nombre del store procedure
                        command.CommandText = $"{prefijo}.SPPDX_GetLinesProduction";  // Se asigna el SP a ejecutar
                        // Creamos la conexión del comando a la conexión creada con HanaConnection
                        command.Connection = connection;
                        // Definimos el comando que se va mandar
                        command.CommandType = CommandType.StoredProcedure;
                        //daCommand.Parameters.Add("Item", HanaDbType.VarChar).Value = Item;
                        using (HanaDataReader myReader = command.ExecuteReader())
                        {
                            if (myReader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(myReader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    //Creamos la variable donde alamacenara la respuesta de Hana
                                    string JSONString = string.Empty;
                                    // Proceso de realizar la serealización la respuesta de Hana
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    // Deserealizamos la respuesta para enviar la respuesta en formato JSON
                                    //Falta hacer el modelo 
                                    var data = JsonConvert.DeserializeObject<List<LineaModel>>(JSONString);
                                    // retornamos la respuesta con estatus 200
                                    return StatusCode(StatusCodes.Status200OK, new { data = data }); // Devolvemos el resultado en JSON
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
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = ex.ToString() }); // Devolvemos el resultado en JSON si no encuentra nada
            }
        }

        [HttpGet]
        [Route("GetPVCScrap")]
        public IActionResult GetPVCScrap()
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    //Abre la conexión a SAP Hana
                    connection.Open();
                    // Se crea la instancia para el ejecutar comandos a Hana
                    using (HanaCommand command = new HanaCommand())
                    {
                        // Creamos el comando a ejecutar en Sap Hana (DBName.NombreStoreProcedure)
                        //Checar el nombre del store procedure
                        command.CommandText = $"{prefijo}.SPPDX_GetPVCScrap";  // Se asigna el SP a ejecutar
                        // Creamos la conexión del comando a la conexión creada con HanaConnection
                        command.Connection = connection;
                        // Definimos el comando que se va mandar
                        command.CommandType = CommandType.StoredProcedure;
                        //daCommand.Parameters.Add("Item", HanaDbType.VarChar).Value = Item;
                        using (HanaDataReader myReader = command.ExecuteReader())
                        {
                            if (myReader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(myReader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    //Creamos la variable donde alamacenara la respuesta de Hana
                                    string JSONString = string.Empty;
                                    // Proceso de realizar la serealización la respuesta de Hana
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    // Deserealizamos la respuesta para enviar la respuesta en formato JSON
                                    //Falta hacer el modelo 
                                    var data = JsonConvert.DeserializeObject<List<InfoPtm>>(JSONString);
                                    // retornamos la respuesta con estatus 200
                                    return StatusCode(StatusCodes.Status200OK, new { data = data }); // Devolvemos el resultado en JSON
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
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = ex.ToString() }); // Devolvemos el resultado en JSON si no encuentra nada
            }
        }

        [HttpGet]
        [Route("GetInjectionScrap")]
        public IActionResult GetInjectionScrap()
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    //Abre la conexión a SAP Hana
                    connection.Open();
                    // Se crea la instancia para el ejecutar comandos a Hana
                    using (HanaCommand command = new HanaCommand())
                    {
                        // Creamos el comando a ejecutar en Sap Hana (DBName.NombreStoreProcedure)
                        //Checar el nombre del store procedure
                        command.CommandText = $"{prefijo}.SPPDX_GetInjectionScrap";  // Se asigna el SP a ejecutar
                        // Creamos la conexión del comando a la conexión creada con HanaConnection
                        command.Connection = connection;
                        // Definimos el comando que se va mandar
                        command.CommandType = CommandType.StoredProcedure;
                        //daCommand.Parameters.Add("Item", HanaDbType.VarChar).Value = Item;
                        using (HanaDataReader myReader = command.ExecuteReader())
                        {
                            if (myReader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(myReader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    //Creamos la variable donde alamacenara la respuesta de Hana
                                    string JSONString = string.Empty;
                                    // Proceso de realizar la serealización la respuesta de Hana
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    // Deserealizamos la respuesta para enviar la respuesta en formato JSON
                                    //Falta hacer el modelo 
                                    var data = JsonConvert.DeserializeObject<List<InfoPtm>>(JSONString);
                                    // retornamos la respuesta con estatus 200
                                    return StatusCode(StatusCodes.Status200OK, new { data = data }); // Devolvemos el resultado en JSON
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
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = ex.ToString() }); // Devolvemos el resultado en JSON si no encuentra nada
            }
        }

        [HttpGet]
        [Route("GetFinishedProduct")]
        public IActionResult GetFinishedProduct()
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    //Abre la conexión a SAP Hana
                    connection.Open();
                    // Se crea la instancia para el ejecutar comandos a Hana
                    using (HanaCommand command = new HanaCommand())
                    {
                        // Creamos el comando a ejecutar en Sap Hana (DBName.NombreStoreProcedure)
                        //Checar el nombre del store procedure
                        command.CommandText = $"{prefijo}.SPPDX_GetFinishedProduct";  // Se asigna el SP a ejecutar
                        // Creamos la conexión del comando a la conexión creada con HanaConnection
                        command.Connection = connection;
                        // Definimos el comando que se va mandar
                        command.CommandType = CommandType.StoredProcedure;
                        //daCommand.Parameters.Add("Item", HanaDbType.VarChar).Value = Item;
                        using (HanaDataReader myReader = command.ExecuteReader())
                        {
                            if (myReader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(myReader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    //Creamos la variable donde alamacenara la respuesta de Hana
                                    string JSONString = string.Empty;
                                    // Proceso de realizar la serealización la respuesta de Hana
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    // Deserealizamos la respuesta para enviar la respuesta en formato JSON
                                    //Falta hacer el modelo 
                                    var data = JsonConvert.DeserializeObject<List<InfoPtm>>(JSONString);
                                    // retornamos la respuesta con estatus 200
                                    return StatusCode(StatusCodes.Status200OK, new { data = data }); // Devolvemos el resultado en JSON
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
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = ex.ToString() }); // Devolvemos el resultado en JSON si no encuentra nada
            }
        }

        [HttpGet]
        [Route("SSHExample")]
        public IActionResult ExampleSSH()
        {
            try
            {
                var sshClient = new SshClient("10.161.60.152", "partner", "7SkyOne*NmNmODg1Mm");
                HanaResponses hana = new HanaResponses();
                sshClient.Connect();

                var portForwarded = new ForwardedPortLocal("localhost", 30015, "hana_host", 30015);
                sshClient.AddForwardedPort(portForwarded);
                portForwarded.Start();

                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_ExampleSP"; // Se asigna el SP a ejecutar
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;

                        var hanaReader = command.ExecuteReader();

                        if (hanaReader.HasRows)
                        {
                            hana.Items = new DataTable();
                            hana.Items.Load(hanaReader);
                            using (hana.Items)
                            {
                                hana.response = new Dictionary<string, string>();
                                //Creamos la variable donde alamacenara la respuesta de Hana
                                string JSONString = string.Empty;
                                // Proceso de realizar la serealización la respuesta de Hana
                                JSONString = JsonConvert.SerializeObject(hana.Items);
                                // Deserealizamos la respuesta para enviar la respuesta en formato JSON
                                //Falta hacer el modelo 
                                var data = JsonConvert.DeserializeObject<List<InfoPtm>>(JSONString);
                                // retornamos la respuesta con estatus 200
                                connection.Close();
                                portForwarded.Stop();
                                sshClient.Disconnect();
                                return StatusCode(StatusCodes.Status200OK, new { data = data }); // Devolvemos el resultado en JSON

                            }
                        } else
                        {
                            connection.Close();
                            portForwarded.Stop();
                            sshClient.Disconnect();
                            hana.response = new Dictionary<string, string>();
                            return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = ex.ToString() }); // Devolvemos el resultado en JSON si no encuentra nada
            }

        }
    }
}
