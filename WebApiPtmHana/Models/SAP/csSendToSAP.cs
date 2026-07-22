using Azure;
using log4net;
using log4net.Core;
using MantenimientosPTM;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Sap.Data.Hana;
using SAPbobsCOM;
using Serilog;
using System.Data;
using System.Text;

namespace PryPTM
{
    class csSendToSAP
    {
        private readonly IConfiguration _configuration;
        public LoginServiceLayer LoginService = new LoginServiceLayer();

        public csSendToSAP()
        {
            // Constructor por compatibilidad: carga appsettings.json desde el directorio base de la aplicación
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            _configuration = builder.Build();
        }

        public csSendToSAP(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string SAPSLDServer { get { return _configuration["SapDBCredentials:SAPSLDServer"]; } }
        string SAPServer { get { return _configuration["SapDBCredentials:SAPServer"]; } }
        string SAPCompanyDB { get { return _configuration["SapDBCredentials:SAPCompanyDB"]; } }
        string SAPUserName { get { return _configuration["SapDBCredentials:SAPUserName"]; } }
        string SAPUserPassword { get { return _configuration["SapDBCredentials:SAPUserPassword"]; } }

        string hanaconnectionString { get { return _configuration.GetConnectionString("Hana"); } }
        string SQLconnectionString { get { return _configuration.GetConnectionString("Sql"); } }


        public string Path_Source;
        public string Path_Processed;
        public string Path_Review;
        public string Alert_Users;
        public string EmailError;

        Company vCmp;
        
        public async Task<GlobalCommands.SapResponse> ReciProdAsync(string sFile, string sIdDBFile, string identificador,string Request)
        {
            var responseAbx = new GlobalCommands.SapResponse { IsError = true };

            try
            {
                Asignar();

                Log.Information("═══════════════════════════════════════════════════");

                Log.Information(
                    "INICIO Goods Receipt - {Request}",
                    Request);

                Log.Information("═══════════════════════════════════════════════════");

                string XmlFile = @"C:\Paradox\PTM\PTMConnect\SourcePath\" + sFile;

                // ✅ 1 — Login
                Log.Information($"🔐 Iniciando sesión en SAP Service Layer...");
                var loginResult = await LoginService.LoginAsyncHttpClient();

               
                Log.Information($"📤 Body armado correctamente");
                Log.Debug($"📤 JSON: {Request}");

                var content = new StringContent(Request, Encoding.UTF8, "application/json");
                content.Headers.ContentType.CharSet = "utf-8";

                // ✅ 4 — Enviar a SAP
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

                    //Actualiza a nivel linea en base de datos SQL Historial Pesadas
                    ActualizaTablaSQLHP(identificador, 1, "OK", responseAbx.DocNum);

                    UpdateXMLStatus(sIdDBFile, responseAbx.DocNum, "Documento Importado a SAP!", "Processed");
                    //SendMessageSAP("Recibo de Produccion", responseAbx.DocEntry, responseAbx.DocNum, sFile, sIdDBFile, "59");
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

                    Log.Error($"❌ Error al crear Purchase Request — StatusCode: {(int)response.StatusCode}");
                    Log.Error($"❌ Detalle: {responseAbx.Message}");
                    //Actualiza a nivel linea en base de datos SQL Historial Pesadas
                    ActualizaTablaSQLHP(identificador, 3, responseAbx.Message, "");
                }
                //Documents oJE;
                //vCmp = new Company();
                //vCmp = ConectarSBO();

                //if (!vCmp.Connected) //Si hubo error al conectar a SAP
                //{
                //    UpdateXMLStatus(sIdDBFile, "", vCmp.GetLastErrorDescription().ToString(), "Error");
                //    InsertError(sFile, "csSendToSAP.ReciProd", vCmp.GetLastErrorDescription().ToString());

                //    //Actualiza a nivel linea en base de datos SQL Historial Pesadas
                //    ActualizaTablaSQLHP(identificador, 3, vCmp.GetLastErrorDescription().ToString(), "");
                //}

                //oJE = vCmp.GetBusinessObject(BoObjectTypes.oInventoryGenEntry); //Tipo de Documento(1)-- Recibo de Producción


                //// Browses XML formatted data and enables to update the data.
                //oJE.Browser.ReadXml(XmlFile, 0);

                //int lRetCode;

                //lRetCode = oJE.Add();

                //if (lRetCode == 0) //Sino no hay errores al exportar a SAP
                //{
                //    string DocEntry;
                //    vCmp.GetNewObjectCode(out DocEntry);

                //    //Obtener docnum de recibo de produccion que se genero en sap
                //    string DocNum = GetDocNum("Recibo de Produccion", DocEntry, sFile, sIdDBFile);

                //    //Actualiza a nivel linea en base de datos SQL Historial Pesadas
                //    ActualizaTablaSQLHP(identificador, 1, "OK", DocNum); 

                //    UpdateXMLStatus(sIdDBFile, DocNum, "Documento Importado a SAP!", "Processed");
                //    SendMessageSAP("Recibo de Produccion", DocEntry, DocNum, sFile, sIdDBFile, "59");
                //    if (vCmp.Connected)
                //    {
                //        vCmp.Disconnect();
                //    }
                //    return string.Empty;
                //}
                //else
                //{
                //    string error = vCmp.GetLastErrorDescription().ToString();
                //    ActualizaTablaSQLHP(identificador, 3, vCmp.GetLastErrorDescription().ToString(), "");
                //    UpdateXMLStatus(sIdDBFile, "", vCmp.GetLastErrorDescription().ToString(), "Error");
                //    if (vCmp.Connected)
                //    {
                //        vCmp.Disconnect();
                //    }
                //    return error;
                //}
                return responseAbx;
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.Append(ex.InnerException != null ? ex.InnerException.ToString() : string.Empty);
                error.Append(ex.Message != null ? ex.Message.ToString() : string.Empty);
                //Actualiza a nivel linea en base de datos SQL Historial Pesadas [insertamos el error en comentariosSAP]
                ActualizaTablaSQLHP(identificador, 3, ex.Message, "");
                UpdateXMLStatus(sIdDBFile, "", ex.Message, "Error");
                InsertError(sFile, "csSendToSAP.ReciProd", ex.Message);
                if(vCmp.Connected)
                {
                    vCmp.Disconnect();
                }
                responseAbx.IsError = true;
                responseAbx.DocNum = string.Empty;
                responseAbx.DocEntry = string.Empty;
                responseAbx.Message = $"No fue posible generar recibo de producción, Error SAP ({ex.Message.ToString()})";
                return responseAbx;
            }

            finally
            {
                // ✅ 5 — Logout siempre
                Log.Information($"🔓 Cerrando sesión SAP...");
                var logoutResult = await LoginService.LogoutAsyncHttpClient();

                if (logoutResult.IsError)
                    Log.Warning($"⚠️ Logout con advertencia: {logoutResult.Message}");
                else
                    Log.Information($"✅ Logout exitoso");

                Log.Information($"🏁 ═══════════════════════════════════════════════════");
                Log.Information($"🏁 FIN Goods Receipt — Request: {Request}");
                Log.Information($"🏁 ═══════════════════════════════════════════════════");
            }
        }

        string GetDocNum(string sDocType, string sDocEntry, string sFile, string sIdDBFile)
        {
            string DocNum = "0";

            try
            {
                HanaDataReader reader;

                // PARAMETROS DEL STORED PROCEDURE
                HanaParameter[] sqls = new HanaParameter[2];

                sqls[0] = new HanaParameter("@vDocType", sDocType);
                sqls[1] = new HanaParameter("@vDocEntry", Convert.ToInt32(sDocEntry));

                // CREO LA CONEXION
                using(HanaConnection myConnection = new HanaConnection(hanaconnectionString))
                {
                    if (myConnection.State == ConnectionState.Closed)
                    {
                        myConnection.Open();
                    }
                    using (HanaCommand cmd = new HanaCommand(SAPCompanyDB + "." + "SPPDXPT_OBTIENEDOCNUMCREADO", myConnection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(sqls);
                        reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            DocNum = reader["DocNum"].ToString();
                        }
                        else
                        {
                            InsertError(sFile, "csSendToSAP.GetDocNum", "No se pudo obtener el DocNum del DocEntry: " + sDocEntry);
                        }
                        myConnection.Close();
                        return DocNum;
                    }
                }
            }
            catch (HanaException ex)
            {
                InsertError(sFile, "csSendToSAP.GetDocNum", ex.Message);
                return "0";
            }
        }


        void SendMessageSAP(string sDocType, string sDocEntry, string sDocNum, string sFile, string sIdDBFile, string sObjType)
        {
            MessagesService oMessageService;
            Message oMessage;
            CompanyService oCmpSrv;

            MessageDataColumns pMessageDataColumns = null;
            MessageDataColumn pMessageDataColumn = null;
            MessageDataLines oLines = null;
            MessageDataLine oLine = null;

            try
            {
                oCmpSrv = vCmp.GetCompanyService();

                oMessageService = (MessagesService) oCmpSrv.GetBusinessService(ServiceTypes.MessagesService);
                oMessage = ((Message)(oMessageService.GetDataInterface(MessagesServiceDataInterfaces.msdiMessage)));

                oMessage.Subject = "(PTM) " + sDocType.ToUpper() + " - IMPORTADO";
                oMessage.Text = "Un nuevo " + sDocType + " ha sido cargado a través de PTM-Connect, da clic en la flecha amarilla de abajo para abrirla.";
                oMessage.Priority = BoMsgPriorities.pr_High;

                int NextUser = 0;
                string[] users = Alert_Users.Split(',');

                foreach (var user in users)
                {
                    oMessage.RecipientCollection.Add();
                    {
                        oMessage.RecipientCollection.Item(NextUser).SendInternal = BoYesNoEnum.tYES;
                        oMessage.RecipientCollection.Item(NextUser).UserCode = user.Trim();
                        oMessage.RecipientCollection.Item(NextUser).UserType = BoMsgRcpTypes.rt_InternalUser;
                    }
                    NextUser++;
                }

                pMessageDataColumns = oMessage.MessageDataColumns;

                pMessageDataColumn = pMessageDataColumns.Add();
                {
                    //Nombre de columna en el panel inferior de la notificación interna de SAP
                    pMessageDataColumn.ColumnName = "Detalles";

                    pMessageDataColumn.Link = BoYesNoEnum.tYES;
                    //get lines
                    oLines = pMessageDataColumn.MessageDataLines;

                    //add new line
                    oLine = oLines.Add();
                    {
                        //Texto de la línea con el enlace al documento
                        oLine.Value = "Enlace a " + sDocType + ": " + sDocNum;
                        //Número de objeto (Factura de Clientes = 13, factura de proveedores = 18)
                        oLine.Object = sObjType;
                        //set the bo code
                        oLine.ObjectKey = sDocEntry;
                    }
                }
                oMessageService.SendMessage(oMessage);
            }
            catch (Exception ex)
            {
                InsertError(sFile, "csSendToSAP.SendMessageSAP", ex.Message);
            }
        }

        public void Asignar()
        {
            Inicializar();

            try
            {
               
                using(HanaConnection conn = new HanaConnection(hanaconnectionString))
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
                            DataTable myDtTbl;
                            myDtTbl = new DataTable();
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
                            if(conn.State == ConnectionState.Open)
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
                            InsertError("PryPTM.sln", "PTMConnect.Globales.Asignar", "La tabla de parametros se encuentra vacia");
                        }
                    }
                }
            }
            catch (HanaException ex)
            {
                InsertError("PryPTM.sln", "PTMConnect.Globales.Asignar", ex.Message);
            }
        }

        public void Inicializar()
        {
            Path_Source = "";
            Path_Processed = "";
            Path_Review = "";
            Alert_Users = "";
            EmailError = "";
        }

        public void UpdateXMLStatus(string sIdXmlFile, string sDocNum, string sResult, string sStatus)
        {
            try
            {
                // PARAMETROS DEL STORED PROCEDURE
                HanaParameter[] sqls = new HanaParameter[4];

                sqls[0] = new HanaParameter("@vIdXmlFile", Convert.ToInt32(sIdXmlFile));
                sqls[1] = new HanaParameter("@vDocNum", sDocNum);
                sqls[2] = new HanaParameter("@vResult", sResult);
                sqls[3] = new HanaParameter("@vStatus", sStatus);

                using(HanaConnection conn = new HanaConnection(hanaconnectionString))
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
                // CREO LA CONEXION
            }
            catch (HanaException ex)
            {
                InsertError("PryPTM.sln", "PTMConnect.UpdateXMLStatus", ex.Message);
            }
        }
        public void InsertError(string sFile, string sLocation, string sMessage)
        {
            try
            {

                // PARAMETROS DEL STORED PROCEDURE
                HanaParameter[] sqls = new HanaParameter[3];

                sqls[0] = new HanaParameter("@vFileName", sFile);
                sqls[1] = new HanaParameter("@vLocation", sLocation);
                sqls[2] = new HanaParameter("@vMessage", sMessage);

                // CREO LA CONEXION
                using(HanaConnection conn = new HanaConnection(hanaconnectionString))
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

                if (sFile != "csSendMail.cs") { } //Solo si el Error no viene del envio del correo(Para no hacer un Loop)

            }
            catch (HanaException ex)
            {
                
            }
        }

        public Company ConectarSBO()
        {
            try
            {

                Company SBO = new Company();
                //Si no hay una conexion existente
                if (!SBO.Connected)
                {
                    SBO = new Company();

                    SBO.LicenseServer = SAPSLDServer;

                    SBO.DbServerType = BoDataServerTypes.dst_HANADB;

                    SBO.Server = SAPServer;

                    SBO.CompanyDB = SAPCompanyDB;

                    SBO.UserName = SAPUserName;

                    SBO.Password = SAPUserPassword;

                    SBO.language = BoSuppLangs.ln_Spanish_La;

                    int result = SBO.Connect();
                    if (result != 0)
                    {
                        string error = SBO.GetLastErrorDescription();
                        return null;
                    }
                    else
                    {
                        if (SBO.Connected == false)
                        {
                            SBO.Connect();
                        }

                        return SBO;
                    }
                }
                else
                {
                    return SBO;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public string SaveXmlFile(string sXmlFileName)
        {
            string Id = "";

            try
            {
                HanaDataReader reader;

                // PARAMETROS DEL STORED PROCEDURE
                HanaParameter[] sqls = new HanaParameter[1];

                sqls[0] = new HanaParameter("@vXmlFileName", sXmlFileName);

                // CREO LA CONEXION
                using(HanaConnection conn = new HanaConnection(hanaconnectionString))
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
                            InsertError("PryPTM.sln", "PTMConnect.SaveXmlFile", "");
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
                InsertError("PryPTM.sln", "PTMConnect.SaveXmlFile", ex.Message);
                return "0";
            }
        }

        //Actualiza la tabla HistorialPesadas en SQL tambien agrega los errores a la columna de comentariosSap para visualizarlos)
        void ActualizaTablaSQLHP(string identificador, int estatusSap, string? descripcion, string Docnum)
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
                        command.Parameters.AddWithValue("@Descripcion", descripcion);
                        command.Parameters.AddWithValue("@Identificador", identificador);
                        command.Parameters.AddWithValue("@EstatuSap", estatusSap);
                        command.Parameters.AddWithValue("@Docnum", Docnum);
                        command.ExecuteNonQuery();
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //funcio para guardar la excepciones del api en "comentariosSAP"
        internal void ActualizaTablaSQLHPCatch(string identificador, int estatusSap, string? descripcion, string Docnum)
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
                        command.Parameters.AddWithValue("@Descripcion", descripcion);
                        command.Parameters.AddWithValue("@Identificador", identificador);
                        command.Parameters.AddWithValue("@EstatuSap", estatusSap);
                        command.Parameters.AddWithValue("@Docnum", Docnum);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
