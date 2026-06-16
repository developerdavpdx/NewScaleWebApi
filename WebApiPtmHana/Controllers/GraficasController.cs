using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using WebApiPtmHana.BLL.Services.ScrapServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models.Graficas;
using WebApiPtmHana.Models.Reportes;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraficasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IScrap _scrap;
        private readonly string connectionString = "";
        private IQueryable<dynamic> skuCountByTurno;

        public GraficasController(ApplicationDbContext context, IConfiguration configuration, IScrap scrap)
        {
            _context = context;
            connectionString = configuration.GetConnectionString("SQLConnection");
            _scrap = scrap;
        }

        /// <summary>
        /// Obtenemos los datos para generar las graficas
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GraficasInfo")]
        public IActionResult GetGraficasInfo([FromHeader] string planta, [FromHeader] string turno,
            [FromHeader] string proceso)
        {
            try
            {
                //definimos los horarios de los turnos (inicio y fin)
                DateTime horaInicioTurno1 = DateTime.Today.AddHours(4).AddMinutes(30).AddSeconds(1); //Inicio del turno 1 (hoy a las 4:30:01 am)
                DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); //Fin del turno 1 (hoy a las 4:30 pm)
                DateTime horaInicioTurno2 = DateTime.Today.AddHours(16).AddMinutes(30).AddSeconds(1); //Inicio del turno 2 (hoy a las 4:30:01 pm)
                DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); //Fin del turno 2 (mañana a las 4:30 am)
                DateTime horaActual = DateTime.Now;

                DateTime horaTurnoStart;
                DateTime horaTurnoEnd;

                //Asignamos el horario del turno inicio y fin de acuerdo al turno actual
                //si el turno es 1
                if (turno == "1")
                {
                    horaTurnoStart = horaInicioTurno1;
                    horaTurnoEnd = horaFinTurno1;
                }
                //si el turno es 2
                else
                {
                    horaTurnoStart = horaInicioTurno2;
                    horaTurnoEnd = horaFinTurno2;
                    //si estamos entre las 00 y las 4:30 am
                    if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                    {
                        horaTurnoStart = horaInicioTurno2.AddDays(-1);
                        horaTurnoEnd = horaFinTurno2.AddDays(-1);
                    }
                }

                string JSONstringSp = string.Empty;
                List<KPISinfo> KPISdatosVolumenes = new List<KPISinfo>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_DatosGraficas";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Turno", int.Parse(turno));
                        command.Parameters.AddWithValue("@FechaTurnoInicio", horaTurnoStart);
                        command.Parameters.AddWithValue("@FechaTurnoFin", horaTurnoEnd);
                        command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                        command.Parameters.AddWithValue("@Proceso", proceso);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable Items = new DataTable();
                                Items.Load(reader);
                                using (Items)
                                {
                                    Dictionary<string, string> response = new Dictionary<string, string>();
                                    JSONstringSp = JsonConvert.SerializeObject(Items);
                                    KPISdatosVolumenes = JsonConvert.DeserializeObject<List<KPISinfo>>(JSONstringSp);

                                    return StatusCode(StatusCodes.Status200OK, new { KPISdatosVolumenes });
                                }
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status200OK, new { JSONstringSp });
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

        [HttpGet]
        [Route("UpdateObjetivosData")]
        public IActionResult UpdateObjetivosData([FromHeader] string planta, [FromHeader] string proceso,
            [FromHeader] string Sobrepeso, [FromHeader] string Scrap)
        {
            try
            {

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_ActualizaObjetivos";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                        command.Parameters.AddWithValue("@Proceso", proceso);

                        // Redondear a 3 decimales antes de enviar al procedimiento almacenado
                        // Convertirmos usando CultureInfo.InvariantCulture para evitar problemas de formato (elimina los puntos decimales)
                        float sobrepesoValue = float.Parse(Sobrepeso, CultureInfo.InvariantCulture);
                        float scrapValue = float.Parse(Scrap, CultureInfo.InvariantCulture);

                        command.Parameters.AddWithValue("@Sobrepeso", sobrepesoValue);
                        command.Parameters.AddWithValue("@Scrap", scrapValue);

                        command.ExecuteNonQuery();
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Datos actualizados correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }
}
