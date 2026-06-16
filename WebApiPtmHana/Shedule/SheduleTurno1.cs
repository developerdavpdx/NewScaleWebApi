using System.Text;
using FluentScheduler;
using Microsoft.Extensions.Configuration;
using WebApiPTMTest.Services;

namespace WebApiPtmHana.Shedule
{
    public class SheduleTurno1 : Registry
    {
        
        public SheduleTurno1()
        {
            try
            {
                Schedule(() => ExecuteJobTurno1()).ToRunEvery(1).Days().At(17, 45);
                Schedule(() => ExecuteJobTurno2()).ToRunEvery(1).Days().At(5, 45);
                //Schedule(() => ExecuteJobTurno1()).ToRunEvery(2).Minutes();
                //Schedule(() => ExecuteJobTurno2()).ToRunEvery(4).Minutes();

                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/CorreJob.txt";
                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    if (!System.IO.File.Exists(rutaEmail))
                    {
                        System.IO.File.Create(rutaEmail).Close();
                    }
                    writer.WriteLine($"{DateTime.Now} - Corre el Job");
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/ErrorJob.txt";
                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    if (!System.IO.File.Exists(rutaEmail))
                    {
                        System.IO.File.Create(rutaEmail).Close();
                    }
                    writer.WriteLine($"{DateTime.Now} - Error de job: {e.Message}");
                    writer.Close();
                }
            }
        }

        public void ExecuteJobTurno1()
        {
            try // Código de la primera tarea programada (turno 1)
            {
                DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
                DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
                MailService mail = new MailService();

                mail.sendReport(1, "PPVC", horaInicioTurno1, horaFinTurno1);
                mail.sendReport(1, "INY", horaInicioTurno1, horaFinTurno1);
                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/CorreoEnvicado.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.Write($"{DateTime.Now} - Correo enviado correctamente Job1");
                    writer.Close();
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

                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/ErroresMail.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.WriteLine($"{DateTime.Now} - Job1 - {Mensaje.ToString()}");
                    writer.Close();
                }
            }

        }

        public void ExecuteJobTurno2()
        {
            // Código de la segunda tarea programada (turno 2)
            try
            {
                DateTime horaInicioTurno2 = DateTime.Today.AddDays(-1).AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
                DateTime horaFinTurno2 = DateTime.Today.AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
                MailService mail = new MailService();

                mail.sendReport(2, "PPVC", horaInicioTurno2, horaFinTurno2);
                mail.sendReport(2, "INY", horaInicioTurno2, horaFinTurno2);
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                int line = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                string Message = string.Empty;
                StringBuilder Mensaje = new StringBuilder();
                Mensaje.Append("Ocurrio un error al enviar el reporte:");
                Mensaje.Append(ex.Message != null ? ex.Message.ToString() : string.Empty);
                Mensaje.Append(ex.InnerException != null ? ex.InnerException.ToString() : string.Empty);
                Mensaje.Append(" line: " + line);

                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/ErroresMail.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.WriteLine($"{DateTime.Now} - Job2 - {Mensaje.ToString()}");
                    writer.Close();
                }
            }
        }
    }
}
