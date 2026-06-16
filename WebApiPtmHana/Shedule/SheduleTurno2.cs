using System.Text;
using Microsoft.Extensions.Configuration;
using WebApiPTMTest.Services;

namespace WebApiPtmHana.Shedule
{
    public class SheduleTurno2 : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;

        public SheduleTurno2(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/logEmai.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.WriteLine($"{DateTime.Now} - Se ejecuta el job 2");
                    writer.Close();
                }
                var now2 = DateTime.Now;
                var nextRunTime2 = new DateTime(now2.Year, now2.Month, now2.Day, 5, 45, 0);

                if (now2 >= nextRunTime2)
                {
                    nextRunTime2 = nextRunTime2.AddDays(1);
                }

                var delay = nextRunTime2 - now2;

                await Task.Delay(delay, stoppingToken);

                await ExecuteJobTurno2();
            }
        }

        public async Task ExecuteJobTurno2()
        {
            // Código de la segunda tarea programada (turno 2)
            try
            {
                DateTime horaInicioTurno2 = DateTime.Today.AddDays(-1).AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
                DateTime horaFinTurno2 = DateTime.Today.AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
                MailService mail = new MailService();

                mail.sendReport(2, "PPVC", horaInicioTurno2, horaFinTurno2);
                mail.sendReport(2, "INY", horaInicioTurno2, horaFinTurno2);
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
                    writer.WriteLine($"{DateTime.Now} - Job2 - {Mensaje.ToString()}");
                    writer.Close();
                }
            }
        }
    }
}
