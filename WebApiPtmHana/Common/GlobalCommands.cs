using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;

namespace MantenimientosPTM
{
    public class GlobalCommands
    {
        private readonly IConfiguration _configuration;

        public GlobalCommands(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region global

        public StringBuilder Excepcion(Exception E, string msg)
        {

            // 6. Obtener el número de línea del error
            int lineNumber = new StackTrace(E, true).GetFrame(0).GetFileLineNumber();

            // 7. Crear un mensaje de error detallado
            StringBuilder sb = new StringBuilder();
            sb.Append(msg);
            sb.Append(E.Message);
            sb.Append($" (Línea: {lineNumber})");

            return sb;
        }

        
        // Método para convertir un objeto a XML
        public string SerializeToXml<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }

        // Método de login asíncrono con webrequest
        public async Task<GlobalCommands.SapResponse> LoginAsyncHttpWebRequest()
        {
            GlobalCommands.SapResponse responseAbx = new GlobalCommands.SapResponse();
            var loginUrl = $"{_configuration["SapSettings:ServiceLayer"]}/Login";
            
            var loginPayload = new
            {
                CompanyDB = _configuration["SapSettings:SapDatabase"],     // Nombre de la base de datos en SAP
                UserName = _configuration["SapSettings:SapUser"],          // Usuario de SAP
                Password = _configuration["SapSettings:SapPassword"],         // Contraseña
                lang = "en-us"             // Idioma preferido de la sesión
            };
            responseAbx.IsError = true;
            // 1. Ignorar errores de SSL (solo para desarrollo)
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; // Usar TLS 1.2

            var request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.Method = "POST";

            request.UseDefaultCredentials = true;
            request.ContentType = "application/json;odata=minimalmetadata;charset=utf8";

            request.KeepAlive = true;
            //     httpWebRequest.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            request.Accept = "application/json;odata=minimalmetadata";
            request.ServicePoint.Expect100Continue = false;
            request.Headers.Add("B1S-WCFCompatible", "true");
            request.Headers.Add("B1S-MetadataWithoutSession", "true");
            request.AllowAutoRedirect = true;
            request.Timeout = 10000000;

            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                var json = JsonConvert.SerializeObject(loginPayload);
                await streamWriter.WriteAsync(json);
                await streamWriter.FlushAsync();
            }

            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseAbx.IsError = false;
                        var result = await reader.ReadToEndAsync();
                        dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                        string SessionId = jsonResponse.SessionId;
                        responseAbx.SessionId = SessionId;
                        // return responseAbx;
                    }
                }
            }
            catch (WebException ex)
            {
                using (var errorResponse = (HttpWebResponse)ex.Response)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = await reader.ReadToEndAsync();
                        Console.WriteLine($"Login failed: {errorText}");
                        responseAbx.IsError = true;
                        responseAbx.Message = errorText;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
                responseAbx.IsError = true;
                responseAbx.Message = ex.Message;
            }

            return responseAbx;
        }
        // Método cerrar sesion asíncrono con webrequest
        public async Task<GlobalCommands.SapResponse> LogoutAsyncHttpWebRequest(string SessionId)
        {
            var responseAbx = new GlobalCommands.SapResponse
            {
                IsError = true
            };

            var logoutUrl = $"{_configuration["SapSettings:ServiceLayer"]}/Logout";
            var request = (HttpWebRequest)WebRequest.Create(logoutUrl);
            //request.Method = "POST";
            request.ContentType = "application/json";
            request.Method = "POST";
            request.KeepAlive = true;
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            request.Accept = "application/jsona";
            request.Headers.Add("Cookie", $"B1SESSION={SessionId}");


            try
            {
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    responseAbx.IsError = false;
                    responseAbx.Message = "Logout exitoso.";
                }

                return responseAbx;
            }
            catch (Exception ex)
            {
                responseAbx.Message = $"Exception: {ex.Message}";
                return responseAbx;
            }
        }

        public Dictionary<string, string> ConvertToParameters(object obj)
        {
            var parameters = new Dictionary<string, string>();

            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var value = prop.GetValue(obj);
                // Convierte el valor a string o "" si es null
                parameters.Add(prop.Name, value?.ToString() ?? "");
            }

            return parameters;
        }
        #endregion

        #region Class

        public class SapResponse
        {
            public string SessionId { get; set; }
            public string Message { get; set; }
            // public string IdRole { get; set; }
            public object JsonRsp { get; set; }
            public string Version { get; set; }
            public bool IsError { get; set; }
            public string RouteId { get; set; }
            public string OrdenVenta { get; set; }
            public string DocNum { get; set; }  // ⬅️ agregar
            public string DocEntry { get; set; }  // ⬅️ agregar
        }
        public class JsonResponseMtto
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
            public JArray DataArray { get; set; }
        }

        #endregion

    }
}