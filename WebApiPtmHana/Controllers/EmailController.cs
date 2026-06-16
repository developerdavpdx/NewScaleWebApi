using System.Data;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.Email;
using System.IO;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly string prefijo;

        public EmailController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaulConnection");
            //Llamamos al prefijo para los stores
            prefijo = configuration["StoredProcedurePrefix"];
        }

        [HttpGet]
        [Route("GetEmails")]
        public async Task<IActionResult> GetEmails()
        {
            try
            {
                
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GETEMAILS";
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
                                    var Sapemail = JsonConvert.DeserializeObject<List<SapEmail>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { Sapemail });
                                } 
                            } else
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new { response ="No existen emails" });
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
        [Route("GetEmailsMolinos")]
        public async Task<IActionResult> GetEmailsMolinos()
        {
            try
            {

                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GETEMAILSMOLINOS";
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
                                    var Sapemail = JsonConvert.DeserializeObject<List<SapEmail>>(JSONString);
                                    return StatusCode(StatusCodes.Status200OK, new { Sapemail });
                                }
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existen emails" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("sendmail")]
        public async Task<IActionResult> EmailSender([FromBody] EmailModel email)
        {
            // ontenerplantilla del correo 
            string filepath = Directory.GetCurrentDirectory() + "/TicketPTM/emailReporteTemplate.html";
            StreamReader str = new(filepath);
            string txttemplate = str.ReadToEnd();
            str.Close();

            txttemplate = txttemplate.Replace("--tipodereporte--", email.tiporeporte);
            var array = JsonConvert.DeserializeObject<string[]>(email.email);

            // Dirección de correo electrónico del remitente
            string fromAddress = "escaner@ptmexico.com";

            // Dirección de correo electrónico del destinatario
            // string toAddress = "franco.maldonado@paradox-et.com";

            //string toAddress = "dutyc198@gmail.com";
            //string toAddress = email.email;

            byte[] archivoPDF = Convert.FromBase64String(email.base64PDF);
            // Construir el objeto MailMessage
            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromAddress, "PTM");

            for(int i = 0; i < array.Length; i++)
            {
                message.To.Add(array[i].ToString().Trim());
            }

            message.Subject = email.tiporeporte;
            message.Body = txttemplate;
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;

            string rutaArchivoPDF = Directory.GetCurrentDirectory() + "/TicketPTM/" + email.tiporeporte + ".pdf";
            System.IO.File.WriteAllBytes(rutaArchivoPDF, archivoPDF);

            // Crear el objeto Attachment
            Attachment attachment = new Attachment(rutaArchivoPDF);
            attachment.ContentId = "pdfemail";
            message.Attachments.Add(attachment);

            // Configurar el cliente SMTP
            SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("escaner@ptmexico.com", "Ptm2018E");

            try
            {
                // Enviar el correo electrónico
                smtpClient.Send(message);

                attachment.Dispose();
                message.Dispose();
                smtpClient.Dispose();

                System.IO.File.Delete(rutaArchivoPDF);

                return StatusCode(StatusCodes.Status200OK, new { response = "Correo Enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "error al envíar el email" });
            }
        }

        [HttpPost]
        [Route("AddMailTxt")]
        public IActionResult AddMailTxt([FromHeader] string correo, [FromHeader] string planta)
        {
            try
            {
                string path = $"C:\\Paradox\\ListaCorreos{planta}.txt";
                string[] mails = System.IO.File.ReadAllLines(path);
                bool emailRepetido = false;
                foreach (var mail in mails)
                {
                    if (mail == correo)
                    {
                        emailRepetido = true;
                    }
                }
                if (!emailRepetido)
                {
                    using (StreamWriter sw = System.IO.File.AppendText(path))
                    {
                        sw.WriteLine(correo);
                        sw.Close();
                    }
                    return StatusCode(StatusCodes.Status200OK, new { message = $"Correo {correo} se ha agregado correctamente." });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Correo Repetido" });
                }

                //return StatusCode(StatusCodes.Status200OK, new { message = $"Correo {correo} se ha agregado correctamente." });
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("RemoveMailTxt")]
        public IActionResult RemoveMailTxt([FromHeader] string correo, [FromHeader] string planta)
        {
            try
            {
                string path = $"C:\\Paradox\\ListaCorreos{planta}.txt";
                List<string> mails = System.IO.File.ReadAllLines(path).ToList();
                bool emailEncontrado = false;
                foreach (var mail in mails)
                {
                    if (mail == correo)
                    {
                        emailEncontrado = true;
                    }
                }
                if (emailEncontrado)
                {
                    mails.Remove(correo);

                    System.IO.File.WriteAllLines(path, mails);

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Correo no encontrado en la lista" });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = $"Correo {correo} se ha eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetMailTxt")]
        public IActionResult GetMailTxt([FromHeader] string planta)
        {
            try
            {
                string path = $"C:\\Paradox\\ListaCorreos{planta}.txt";
                List<string> mails = System.IO.File.ReadAllLines(path).ToList();

                if (mails.Count == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "No hay correos en el archivo txt" });

                return StatusCode(StatusCodes.Status200OK, new { message = mails });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
    }
}
