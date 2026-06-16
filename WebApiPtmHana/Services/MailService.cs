using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace WebApiPTMTest.Services
{
    public class MailService
    {
        
        public void sendReport(int turno, string proceso, DateTime fechaTurnoInicio, DateTime fechaTurnoFin)
        {
            try
            {

                var atadosTotales = 0;
                var tubosTotales = 0;
                decimal pesoNetoTotales = 0;
                decimal pUnitEstaTotales = 0;
                decimal sobrepesoTotales = 0;
                decimal scrapTotales = 0;
                decimal eficienciaTotales = 0;
                decimal PTotalEstandar = 0;     //<<--- peso total estandar

                string filepath = Directory.GetCurrentDirectory() + "/TicketPTM/ReportEmail.html";
                StreamReader str = new(filepath);
                string txttemplate = str.ReadToEnd();
                str.Close();


                EmailServices _funciones = new EmailServices();
                var datosReport = _funciones.ObtenerDatosReporte(turno, proceso, fechaTurnoInicio, fechaTurnoFin);
                CultureInfo cultureInfo = new CultureInfo("es-MX");

                string tablebody = string.Empty;
                decimal a = 0;
                foreach (var dato in datosReport)
                {
                    atadosTotales = atadosTotales + dato.AtadosTarimas;
                    tubosTotales = tubosTotales + dato.NumTubos;
                    pesoNetoTotales = pesoNetoTotales + dato.PesoTotal;
                    pUnitEstaTotales = pUnitEstaTotales + dato.PUnitEstandar;
                    //sobrepesoTotales = sobrepesoTotales + (dato.SobrePeso / 100);    //deprecado, solo suma los sobrepesos y no aplica formula
                    scrapTotales = scrapTotales + (dato.Scrap /100);
                    eficienciaTotales = eficienciaTotales + (dato.Eficiencia /100);
                    PTotalEstandar = PTotalEstandar + (dato.NumTubos * dato.PUnitEstandar);  //<<-- Calcular el Peso Total Estandar
                    a = dato.PesoTotal;
                                //tabla de resultados (cuerpo)
                    string tr = "<tr>" +
                                $"<td>{dato.Id_Linea}</td>" +
                                $"<td>{dato.Codigo}</td>" +
                                $"<td>{dato.AtadosTarimas}</td>" +
                                $"<td>{dato.NumTubos}</td>" +
                                $"<td>{dato.PesoTotal.ToString("C", cultureInfo).Replace("$", "")} Kg</td>" +
                                $"<td>{dato.PUnitEstandar.ToString("C", cultureInfo).Replace("$", "")}</td>" +
                                $"<td>{dato.PUnitReal.ToString("C", cultureInfo).Replace("$", "")}</td>" +
                                $"<td>{dato.SobrePeso.ToString("C", cultureInfo).Replace("$", "")} %</td>" +
                                $"<td>{dato.Scrap.ToString("C", cultureInfo).Replace("$", "")} %</td>" +
                                $"<td>{dato.Eficiencia.ToString("C", cultureInfo).Replace("$", "")} %</td>" +
                                // Agrega más columnas según los parámetros necesarios
                                "</tr>";
                    tablebody += tr;
                }

                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/CorreoEnvicado.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.Write($"Valor: {a}");
                    writer.Close();
                }

                                //Tabla de Totales
                sobrepesoTotales = ((pesoNetoTotales / PTotalEstandar) - 1) * 100;      //<<-- Calculo del sobrepeso
                string trTotales = "<tr class='trTotales'>" +
                                $"<td colspan='2'>Totales</td>" +
                                $"<td>{atadosTotales}</td>" +
                                $"<td>{tubosTotales}</td>" +
                                $"<td>{pesoNetoTotales.ToString("C", cultureInfo).Replace("$", "")} Kg</td>" +
                                $"<td>{pUnitEstaTotales.ToString("C", cultureInfo).Replace("$", "")}</td>" +
                                $"<td></td>" +
                                $"<td>{(sobrepesoTotales * 100).ToString("C", cultureInfo).Replace("$", "")} %</td>" +
                                $"<td>{(scrapTotales * 100).ToString("C", cultureInfo).Replace("$", "")} %</td>" +
                                $"<td>{(eficienciaTotales * 100).ToString("C", cultureInfo).Replace("$", "")} %</td>" +
                                // Agrega más columnas según los parámetros necesarios
                                "</tr>";
                tablebody += trTotales;

                if(turno == 1)
                {
                    txttemplate = txttemplate.Replace("{saludoTiempo}", "Buena tarde,");
                    txttemplate = txttemplate.Replace("{turnoInicio}", $"{fechaTurnoInicio.ToString("dd/MM/yyyy HH:mm")}");
                    txttemplate = txttemplate.Replace("{turnoFin}", $"{fechaTurnoFin.ToString("dd/MM/yyyy HH:mm")}");
                } else
                {
                    txttemplate = txttemplate.Replace("{saludoTiempo}", "Buen día,");
                    txttemplate = txttemplate.Replace("{turnoInicio}", $"{fechaTurnoInicio.ToString("dd/MM/yyyy HH:mm")}");
                    txttemplate = txttemplate.Replace("{turnoFin}", $"{fechaTurnoFin.ToString("dd/MM/yyyy HH:mm")}");
                }

                txttemplate = txttemplate.Replace("{proceso}", proceso);
                txttemplate = txttemplate.Replace("{numturno}", $"{turno}");
                txttemplate = txttemplate.Replace("{tablebody}", tablebody);
                txttemplate = txttemplate.Replace("{tiporeporte}", proceso.ToUpper());


                MailMessage message = new MailMessage();
                message.From = new MailAddress("soporte@paradox-et.com", "PTM");
                //message.From = new MailAddress("franco.maldonado@paradox-et.com", "PTM");

                message.Subject = $"Reporte: {proceso}";
                message.Body = txttemplate;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
                message.To.Add("saavedrasi@ptmexico.com");
                message.To.Add("jimenezml@ptmexico.com");
                message.To.Add("soporte@paradox-et.com");
                message.To.Add("franco.maldonado@paradox-et.com");

                // Configurar el cliente SMTP
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                //smtpClient.Credentials = new System.Net.NetworkCredential("franco.maldonado@paradox-et.com", "Paradox#2023");
                smtpClient.Credentials = new System.Net.NetworkCredential("soporte@paradox-et.com", "Paradox#2023");

                try
                {
                    // Enviar el correo electrónicoon
                    smtpClient.Send(message);
                    message.Dispose();
                    smtpClient.Dispose();
                }
                catch (Exception ex)
                {
                    message.Dispose();
                    smtpClient.Dispose();
                }
            } catch(Exception ex)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                int line = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                string Message = string.Empty;
                StringBuilder Mensaje = new StringBuilder();
                Mensaje.Append("Ocurrio un error al enviar el reporte:");
                Mensaje.Append(ex.Message != null ? ex.Message.ToString() : string.Empty);
                Mensaje.Append(ex.InnerException != null ? ex.InnerException.ToString() : string.Empty);
                Mensaje.Append(" line: " + line);

                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/ErroresEnvioMail.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.WriteLine($"{DateTime.Now} - {Mensaje.ToString()}");
                    writer.Close();
                }
            }
        }
    }
}
