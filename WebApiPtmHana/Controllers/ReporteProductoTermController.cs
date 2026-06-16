using System.Data;
using System.Numerics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.ProductoTServ;
using WebApiPtmHana.BLL.Services.ScrapServ;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.Reportes;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteProductoTermController : ControllerBase
    {
        private readonly IProductoTerminado _productTerm;
        private readonly IScrap _scrap;
        private readonly string connectionString;
        private readonly string connectionStringSQL;
        public ReporteProductoTermController(IProductoTerminado productTerm, IConfiguration configuration, IScrap scrap)
        {
            _productTerm = productTerm;
            _scrap = scrap;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            connectionStringSQL = configuration.GetConnectionString("SQLConnection");
        }


        /// <summary>
        /// obtenemos los totales default para inyeccion o ppvc
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllPt")]
        public IActionResult GetAllPt([FromHeader] string planta, [FromHeader] string proceso)
        {
            try
            {
                int numplanta = int.Parse(planta);
                int turno = 0;
                DateTime TurnoStar;
                DateTime TurnoEnd;
                DateTime TurnoScrapStar;
                DateTime TurnoScrapEnd;

                List<InfoOwor> JsonOwor = new List<InfoOwor>();

                DateTime horaInicioTurno = DateTime.Today.AddHours(4.5).AddSeconds(1); // Fecha y hora de inicio del turno 1 (hoy a las 4:30:01 am)
                DateTime horaFinTurno = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30:00 pm)
                DateTime horaActual = DateTime.Now;               
                

                string JSONstringSp = string.Empty;
                List<ReportesProdTerm> reportesProdTerm = new List<ReportesProdTerm>();

                //asignamos el turno de acuerdo al horario y obtenemos los horarios
                if (horaActual >= horaInicioTurno && horaActual <= horaFinTurno)
                {
                    turno = 1;

                    TurnoStar = DateTime.Today.AddHours(4.5).AddSeconds(1); // Fecha y hora de inicio del turno 1 (hoy a las 4:30:01 am)
                    TurnoEnd  = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30:00 pm)

                    TurnoScrapStar = DateTime.Today.AddHours(5).AddMinutes(45).AddSeconds(1); // Fecha y hora de inicio del turno 1 (hoy a las 5:45:01 am)
                    TurnoScrapEnd = DateTime.Today.AddHours(17).AddMinutes(45); // Fecha y hora de fin del turno 1 (hoy a las 5:45:00 pm)
                }
                else 
                {
                    turno = 2;

                    TurnoStar = DateTime.Today.AddHours(16.5).AddSeconds(1); // Fecha y hora de inicio del turno 2 (hoy a las 4:30:01 pm)
                    TurnoEnd = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30:00 am)

                    TurnoScrapStar = DateTime.Today.AddHours(17).AddMinutes(45).AddSeconds(1); // Fecha y hora de inicio del turno 2 (hoy a las 5:45:01 pm)
                    TurnoScrapEnd = DateTime.Today.AddDays(1).AddHours(5).AddMinutes(45); // Fecha y hora de fin del turno 2 (mañana a las 5:45:00 am)

                    //si es el turno 2 y ya es de madrugada elejimos la horainicio de ayer, y hora fin de hoy
                    if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                    {
                        TurnoEnd = TurnoEnd.AddDays(-1);
                        TurnoStar = TurnoStar.AddDays(-1);

                        TurnoScrapStar = TurnoScrapStar.AddDays(-1);
                        TurnoScrapEnd = TurnoScrapEnd.AddDays(-1);
                    }
                }

                    double horas = (horaActual - horaInicioTurno).TotalHours;
                    int horasT = Convert.ToInt32(horas);
                    using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                    {
                        if (cnn.State == ConnectionState.Closed)
                        {
                            cnn.Open();
                        }
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = cnn;
                            command.CommandText = "Sppdx_ObtenerProductosTerminados";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@turno", turno);
                            command.Parameters.AddWithValue("@proceso", proceso);
                            command.Parameters.AddWithValue("@FechaTurnoInicio", TurnoStar);
                            command.Parameters.AddWithValue("@FechaTurnoFin", TurnoEnd);
                            command.Parameters.AddWithValue("@Horas", horasT == 0 ? 1 : horasT);
                            command.Parameters.AddWithValue("@Planta", numplanta);
                            //parametros para las columnas de scrap
                            command.Parameters.AddWithValue("@FechaTurnoInicioScrap", TurnoScrapStar);
                            command.Parameters.AddWithValue("@FechaTurnoFinScrap", TurnoScrapEnd);

                            using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                            using (DataSet dtDocuments = new DataSet())
                            {
                                daCommand.Fill(dtDocuments);
                                DataTable tabla1 = dtDocuments.Tables[0];

                                using (tabla1)
                                {
                                    JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                    reportesProdTerm = JsonConvert.DeserializeObject<List<ReportesProdTerm>>(JSONstringSp);
                                }
                            }

                            return StatusCode(StatusCodes.Status200OK, new { reportesProdTerm});
                        }
                    }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { ex.Message });
            }
        }
    }
}
