using MantenimientosPTM;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Sap.Data.Hana;
using Serilog;
using System.Data;
using System.Text;

namespace WebApiPtmHana.Models.SAP
{
    /// <summary>
    /// SAP Service Layer Integration for ASP.NET Core
    /// </summary>
    public class CSSendToSAP
    {
        private readonly IConfiguration _configuration;
        public LoginServiceLayer LoginService;

        // Configuration properties - read from appsettings.json
        string SAPSLDServer { get { return _configuration["SapDBCredentials:SAPSLDServer"]; } }
        string SAPServer { get { return _configuration["SapDBCredentials:SAPServer"]; } }
        string SAPCompanyDB { get { return _configuration["SapDBCredentials:SAPCompanyDB"]; } }
        string SAPUserName { get { return _configuration["SapDBCredentials:SAPUserName"]; } }
        string SAPUserPassword { get { return _configuration["SapDBCredentials:SAPUserPassword"]; } }

        string hanaconnectionString { get { return _configuration.GetConnectionString("Hana"); } }
        string SQLconnectionString { get { return _configuration.GetConnectionString("Sql"); } }

        // Public properties for file paths
        public string Path_Source;
        public string Path_Processed;
        public string Path_Review;
        public string Alert_Users;
        public string EmailError;

        /// <summary>
        /// Constructor with default configuration loading
        /// </summary>
        public CSSendToSAP()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
            LoginService = new LoginServiceLayer();
        }

        /// <summary>
        /// Constructor with injected configuration (preferred for DI scenarios)
        /// </summary>
        public CSSendToSAP(IConfiguration configuration)
        {
            _configuration = configuration;
            LoginService = new LoginServiceLayer();
        }

        /// <summary>
        /// Async method to send Goods Receipt (Recibo de Producción) to SAP Service Layer
        /// This is the main integration point - uses HttpClient instead of SAPbobsCOM SDK
        /// </summary>
        public async Task<GlobalCommands.SapResponse> ReciProdAsync(string sFile, string sIdDBFile, string identificador, string Request)
        {
            var responseAbx = new GlobalCommands.SapResponse { IsError = true };

            try
            {
                Asignar();

                Log.Information("═══════════════════════════════════════════════════");
                Log.Information("INICIO Goods Receipt - {Request}", Request);
                Log.Information("═══════════════════════════════════════════════════");

                // ✅ 1 — Login
                Log.Information($"🔐 Iniciando sesión en SAP Service Layer...");
                var loginResult = await LoginService.LoginAsyncHttpClient();

                Log.Information($"📤 Body armado correctamente");
                Log.Debug($"📤 JSON: {Request}");

                var content = new StringContent(Request, Encoding.UTF8, "application/json");
                content.Headers.ContentType.CharSet = "utf-8";

                // ✅ 2 — Enviar a SAP
                var url = $"{_configuration["SapSettings:ServiceLayer"]}/InventoryGenEntries";
                Log.Information($"🌐 Enviando POST a: {url}");

                var response = await LoginService._httpClient.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                Log.Information($"📥 Respuesta SAP — StatusCode: {(int)response.StatusCode}");
                Log.Information($"📥 Respuesta SAP — Body: {result}");

                if (response.IsSuccessStatusCode)
                {
                    dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                    responseAbx.IsError = false;
                    responseAbx.DocNum = jsonResponse.DocNum?.ToString() ?? string.Empty;
                    responseAbx.DocEntry = jsonResponse.DocEntry?.ToString() ?? string.Empty;
                    responseAbx.Message = "Recibo de producción creado correctamente en SAP.";

                    Log.Information($"✅ Recibo de producción creado correctamente en SAP — DocNum: {responseAbx.DocNum} | DocEntry: {responseAbx.DocEntry}");

                    // Actualiza a nivel línea en base de datos SQL Historial Pesadas
                    ActualizaTablaSQLHP(identificador, 1, "OK", responseAbx.DocNum);
                    UpdateXMLStatus(sIdDBFile, responseAbx.DocNum, "Documento Importado a SAP!", "Processed");
                }
                else
                {
                    try
                    {
                        dynamic errorResponse = JsonConvert.DeserializeObject(result);
                        string errorCode = errorResponse.error.code?.ToString() ?? "?";
                        string errorValue = errorResponse.error.message.value?.ToString() ?? result;
                        responseAbx.IsError = true;
                        responseAbx.DocNum = string.Empty;
                        responseAbx.DocEntry = string.Empty;
                        responseAbx.Message = $"No fue posible generar recibo de producción, Error SAP: {errorCode} / {errorValue}";
                    }
                    catch
                    {
                        // Si no se puede parsear, mostrar el raw
                        responseAbx.IsError = true;
                        responseAbx.DocNum = string.Empty;
                        responseAbx.DocEntry = string.Empty;
                        responseAbx.Message = $"No fue posible generar recibo de producción, Error SAP ({(int)response.StatusCode}): {result}";
                    }

                    Log.Error($"❌ Error al crear Recibo de Producción — StatusCode: {(int)response.StatusCode}");
                    Log.Error($"❌ Detalle: {responseAbx.Message}");
                    ActualizaTablaSQLHP(identificador, 3, responseAbx.Message, "");
                }

                return responseAbx;
            }
            catch (Exception ex)
            {
                Log.Error($"❌ EXCEPTION en ReciProdAsync: {ex.Message}");
                responseAbx.IsError = true;
                responseAbx.Message = $"Excepción: {ex.Message}";
                ActualizaTablaSQLHP(identificador, 3, ex.Message, "");
                InsertError(sFile, "CSSendToSAP.ReciProdAsync", ex.Message);
                return responseAbx;
            }
        }

        /// <summary>
        /// Initialize configuration values from HANA stored procedure
        /// Loads Path_Source, Path_Processed, Path_Review, etc.
        /// </summary>
        public void Asignar()
        {
            Inicializar();

            try
            {
                using (HanaConnection conn = new HanaConnection(hanaconnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (HanaCommand myCommand = new HanaCommand(SAPCompanyDB + "." + "SPPDXPT_ObtenerParametros", conn))
                    {
                        myCommand.CommandType = CommandType.StoredProcedure;
                        HanaDataReader myReader = myCommand.ExecuteReader();

                        if (myReader.HasRows)
                        {
                            DataTable myDtTbl = new DataTable();
                            myDtTbl.Load(myReader);
                            for (int numRow = 0; numRow <= myDtTbl.Rows.Count - 1; numRow++)
                            {
                                string gloVar = myDtTbl.Rows[numRow]["Variable"].ToString();

                                switch (gloVar)
                                {
                                    case "Path_Source": Path_Source = myDtTbl.Rows[numRow]["Valor"].ToString(); break;
                                    case "Path_Processed": Path_Processed = myDtTbl.Rows[numRow]["Valor"].ToString(); break;
                                    case "Path_Review": Path_Review = myDtTbl.Rows[numRow]["Valor"].ToString(); break;
                                    case "Alert_Users": Alert_Users = myDtTbl.Rows[numRow]["Valor"].ToString(); break;
                                    case "EmailError": EmailError = myDtTbl.Rows[numRow]["Valor"].ToString(); break;
                                }
                            }
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                        else
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                            InsertError("WebApiPtmHana.csproj", "CSSendToSAP.Asignar", "La tabla de parametros se encuentra vacia");
                        }
                    }
                }
            }
            catch (HanaException ex)
            {
                InsertError("WebApiPtmHana.csproj", "CSSendToSAP.Asignar", ex.Message);
            }
        }

        /// <summary>
        /// Reset all configuration variables to empty strings
        /// </summary>
        public void Inicializar()
        {
            Path_Source = "";
            Path_Processed = "";
            Path_Review = "";
            Alert_Users = "";
            EmailError = "";
        }

        /// <summary>
        /// Update XML file processing status in HANA database
        /// </summary>
        public void UpdateXMLStatus(string sIdXmlFile, string sDocNum, string sResult, string sStatus)
        {
            try
            {
                HanaParameter[] sqls = new HanaParameter[4];
                sqls[0] = new HanaParameter("@vIdXmlFile", Convert.ToInt32(sIdXmlFile));
                sqls[1] = new HanaParameter("@vDocNum", sDocNum);
                sqls[2] = new HanaParameter("@vResult", sResult);
                sqls[3] = new HanaParameter("@vStatus", sStatus);

                using (HanaConnection conn = new HanaConnection(hanaconnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (HanaCommand cmd = new HanaCommand(SAPCompanyDB + "." + "SpPdxPT_ActualizaStatusXMLFile", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(sqls);
                        cmd.ExecuteNonQuery();
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (HanaException ex)
            {
                InsertError("WebApiPtmHana.csproj", "CSSendToSAP.UpdateXMLStatus", ex.Message);
            }
        }

        /// <summary>
        /// Insert error log in HANA database
        /// </summary>
        public void InsertError(string sFile, string sLocation, string sMessage)
        {
            try
            {
                HanaParameter[] sqls = new HanaParameter[3];
                sqls[0] = new HanaParameter("@vFileName", sFile);
                sqls[1] = new HanaParameter("@vLocation", sLocation);
                sqls[2] = new HanaParameter("@vMessage", sMessage);

                using (HanaConnection conn = new HanaConnection(hanaconnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (HanaCommand cmd = new HanaCommand(SAPCompanyDB + "." + "SpPdxPT_InsertaError", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(sqls);
                        cmd.ExecuteNonQuery();
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (HanaException ex)
            {
                // Error inserting error log - do nothing to avoid infinite loop
            }
        }

        /// <summary>
        /// Save XML file metadata in HANA database
        /// Returns the generated ID or "0" on failure
        /// </summary>
        public string SaveXmlFile(string sXmlFileName)
        {
            string Id = "";

            try
            {
                HanaDataReader reader;
                HanaParameter[] sqls = new HanaParameter[1];
                sqls[0] = new HanaParameter("@vXmlFileName", sXmlFileName);

                using (HanaConnection conn = new HanaConnection(hanaconnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (HanaCommand cmd = new HanaCommand(SAPCompanyDB + "." + "SpPdxPT_InsertaXmlFile", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(sqls);
                        reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            Id = reader["Id"].ToString();
                        }
                        else
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                            InsertError("WebApiPtmHana.csproj", "CSSendToSAP.SaveXmlFile", "No rows returned");
                            return "0";
                        }
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                        return Id;
                    }
                }
            }
            catch (SqlException ex)
            {
                InsertError("WebApiPtmHana.csproj", "CSSendToSAP.SaveXmlFile", ex.Message);
                return "0";
            }
        }

        /// <summary>
        /// Update HistorialPesadas table in SQL Server
        /// Logs SAP operation status and responses
        /// </summary>
        private void ActualizaTablaSQLHP(string identificador, int estatusSap, string descripcion, string Docnum)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(SQLconnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "Sppdx_ActualizarSapCarga";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Descripcion", descripcion ?? "");
                        command.Parameters.AddWithValue("@Identificador", identificador);
                        command.Parameters.AddWithValue("@EstatuSap", estatusSap);
                        command.Parameters.AddWithValue("@Docnum", Docnum ?? "");
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error in ActualizaTablaSQLHP: {ex.Message}");
            }
        }
    }
}
