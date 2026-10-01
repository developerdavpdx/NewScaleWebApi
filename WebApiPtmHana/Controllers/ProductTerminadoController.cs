using System.Data;
using System.Diagnostics;
using System.Numerics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sap.Data.Hana;
using SAPbobsCOM;
using WebApiPtmHana.BLL.Services.LineasServ;
using WebApiPtmHana.BLL.Services.ProductoTServ;
using WebApiPtmHana.BLL.Services.ScrapServ;
using WebApiPtmHana.BLL.Services.SkuInfoServ;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.Reportes;
using WebApiPtmHana.Models.Scrap;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTerminadoController : ControllerBase
    {
        private readonly IProductoTerminado _productoTerminado;
        //private readonly IProductoTerminado _productTerm;
        private readonly ISkuInfo _skuInformation;
        private readonly IConfiguration _configuration;
        private readonly ILinia _lineas;
        private readonly IScrap _scrap;
        //TokensService _tokenService = new TokensService();
        private readonly string connectionString;
        private readonly string connectionStringSQL;

        public class UpdateMaterialRequest
        {
            public string Id { get; set; }
            public string MultiploCant { get; set; }
            public string MultiploCat { get; set; }
            public string Variable { get; set; }
            public string ItemName { get; set; }
            public string MinWeight { get; set; }
            public string FlejeCant { get; set; }
            public string AnilloCant { get; set; }
            public string MaderaCant { get; set; }
            public string ArpillaCant { get; set; }
            public string CostalesCant { get; set; }
            public string TarimaCant { get; set; }
            public string Planta { get; set; }
            public string Usuario { get; set; }
            public string UserId { get; set; }
        }

        public ProductTerminadoController(IProductoTerminado productoTerminado, ISkuInfo skuInformation, IConfiguration configuration,
            ILinia lineas, IScrap scrap)
        {
            _productoTerminado = productoTerminado;
            _skuInformation = skuInformation;
            _configuration = configuration;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            connectionStringSQL = configuration.GetConnectionString("SQLConnection");
            _lineas = lineas;
            _scrap = scrap;
        }

        [HttpPost]
        [Route("UpdateMaterialEmb")]
        public async Task<IActionResult> UpdateMaterialEmb([FromBody] UpdateMaterialRequest request)
        {
            try
            {
                int numplanta = int.Parse(request.Planta);
                // Procesa los datos aquí
                bool result = await _skuInformation.UpdateSkuById(request.Id, (int)float.Parse(request.MultiploCant), request.MultiploCat,
                    float.Parse(request.Variable), request.ItemName, float.Parse(request.MinWeight),
                    float.Parse(request.FlejeCant), float.Parse(request.AnilloCant), float.Parse(request.MaderaCant),
                    float.Parse(request.ArpillaCant), float.Parse(request.CostalesCant), float.Parse(request.TarimaCant),
                    numplanta, request.Usuario);

                if (result)
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = "SKU actualizado correctamente" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No se ha podido realizar la actualización" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }



        [HttpGet]
        [Route("GetLineas")]
        public IActionResult GetLineas([FromHeader] string planta)
        {
            IList<Lineas> lineas = _lineas.CountLineas(planta);
            return StatusCode(StatusCodes.Status200OK, new { response = lineas });
        }

        //obtener los detalles por linea para PPVC
        [HttpGet]
        [Route("GetPTByLinie")]
        public IActionResult GetPTPvcByLinie([FromHeader] string numLinea, [FromHeader] string planta, [FromHeader] string corte,
            [FromHeader] string filtroDefault, [FromHeader] string fechaInicioFiltro, [FromHeader] string fechaFinFiltro,
            [FromHeader] string codigo, [FromHeader] string proceso)
        {
            try
            {
                //Si no se ha usado el filtrado de turnos/dias <<--
                if (filtroDefault == "0")
                {
                    List<ReportesDetalle> ReportesDetalle = new List<ReportesDetalle>();
                    int turno = 0;

                    DateTime horaInicio = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
                    DateTime horaFin = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
                    //obtenemos la hora actual
                    DateTime horaActual = DateTime.Now;

                    //seleccionamos el turno (y cambiamos las horas de inicio/fin si es el turno 2)
                    if (horaActual >= horaInicio && horaActual <= horaFin)
                    {
                        turno = 1;
                    }
                    else
                    {                                                            
                        turno = 2;
                        horaFin = horaInicio.AddDays(1); // el dia que termina el turno 2 pero a las 04:30
                        horaInicio = horaInicio.AddHours(12); // el dia que comezo el turno 1 pero a las 16:30

                        //si es el turno 2 y ya es de madrugada elejimos la horainicio de ayer, y hora fin de hoy
                        if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                        {
                            horaFin = horaFin.AddDays(-1);
                            horaInicio = horaInicio.AddDays(-1);
                        }
                        
                    }

                    //ejecutamos el procedure en sql
                    using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                    {
                        if (cnn.State == ConnectionState.Closed)
                        {
                            cnn.Open();
                        }
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = cnn;
                            command.CommandText = "Sppdx_ObtenerPTPorLinea";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@turno", turno);
                            command.Parameters.AddWithValue("@proceso", proceso);
                            command.Parameters.AddWithValue("@FechaTurnoInicio", horaInicio);
                            command.Parameters.AddWithValue("@FechaTurnoFin", horaFin);
                            command.Parameters.AddWithValue("@Linea", numLinea);
                            command.Parameters.AddWithValue("@planta", planta);
                            command.Parameters.AddWithValue("@codigo", codigo);
                            command.Parameters.AddWithValue("@corte", corte);
                            command.Parameters.AddWithValue("@filtroDefault", filtroDefault);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                string JSONstringSp = string.Empty;
                                if (reader.HasRows)
                                {
                                    DataTable table = new DataTable();
                                    table.Load(reader);
                                    using (table)
                                    {
                                        JSONstringSp = JsonConvert.SerializeObject(table);
                                        ReportesDetalle = JsonConvert.DeserializeObject<List<ReportesDetalle>>(JSONstringSp);

                                        return StatusCode(StatusCodes.Status200OK, new { ReportesDetalle });
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
                //si Se ah usado un filtro <<--
                else
                {
                    List<ReportesDetalle> ReportesDetalle = new List<ReportesDetalle>();
                    int turno = 0;

                    string horaInicio = "";
                    string horaFin = "";

                    //Si el cortes es diferente de 0, se esta seleccionando alguno de los cortes, de un solo dia
                    if (corte != "0")
                    {
                        //usamos el corte seleccionado para asignar los horarios y luego reasignamos valor a corte para usar en sql
                        switch (corte.ToString())
                        {
                            case "1": //corte 1 turno 1
                                horaInicio = $"{fechaInicioFiltro} 04:30:01";
                                horaFin = $"{fechaInicioFiltro.Replace("04:30:01", "")} 11:00:00";
                                corte = "1";
                                turno = 1;
                                break;
                            case "2": //corte 2 turno 1
                                horaInicio = $"{fechaInicioFiltro} 11:00:01";
                                horaFin = $"{fechaInicioFiltro.Replace("11:00:01", "")} 16:30:00";
                                corte = "2";
                                turno = 1;
                                break;
                            case "3": //Todo el turno 1
                                horaInicio = $"{fechaInicioFiltro} 04:30:01";
                                horaFin = $"{fechaInicioFiltro.Replace("04:30:01", "")} 16:30:00";
                                corte = "3"; //este 3 se asigna para indicar que es todo el turno en sql
                                turno = 1;
                                break;
                            case "4": //corte 1 turno 2
                                horaInicio = $"{fechaInicioFiltro} 16:30:01";
                                horaFin = $"{fechaInicioFiltro.Replace("16:30:01", "")} 23:00:00";
                                corte = "1";
                                turno = 2;
                                break;
                            case "5": //corte 2 turno 2
                                horaInicio = $"{fechaInicioFiltro} 23:00:01";
                                horaFin = $"{DateTime.Parse(fechaInicioFiltro).AddDays(1):yyyy/MM/dd} 04:30:00";
                                corte = "2";
                                turno = 2;
                                break;
                            case "6": //Todo el turno 2
                                horaInicio = $"{fechaInicioFiltro} 16:30:01";
                                horaFin = $"{DateTime.Parse(fechaInicioFiltro).AddDays(1):yyyy/MM/dd} 04:30:00";
                                corte = "3"; //este 3 se asigna para indicar que es todo el turno en sql
                                turno = 2;
                                break;
                            case "7": //Reporte del dia (turno 1 y 2)
                                horaInicio = $"{fechaInicioFiltro} 04:30:01";
                                horaFin = $"{DateTime.Parse(fechaInicioFiltro).AddDays(1):yyyy/MM/dd} 04:30:00";
                                corte = null; // mandamos corte como null para indicar que es reporte del dia
                                turno = 3; //con el turno 3 indicamos que es Reporte del dia
                                break;
                        };
                    }

                    //si el corte es 0, se esta seleccionando un rango de dias
                    if (corte == "0")
                    {
                        horaInicio = $"{fechaInicioFiltro} 04:30:00"; ; // Fecha y hora de inicio del turno 1
                        horaFin = $"{DateTime.Parse(fechaFinFiltro).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00"; //el dia seleccionado como fin, mas 1 dia
                        turno = 3; //Reporte del dia o en este caso, rango de dias
                        corte = "4";
                    }

                    //ejecutamos el procedure en sql
                    using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                    {
                        if (cnn.State == ConnectionState.Closed)
                        {
                            cnn.Open();
                        }
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = cnn;
                            command.CommandText = "Sppdx_ObtenerPTPorLinea";
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@turno", turno);
                            command.Parameters.AddWithValue("@proceso", proceso);
                            command.Parameters.AddWithValue("@FechaTurnoInicio", horaInicio);
                            command.Parameters.AddWithValue("@FechaTurnoFin", horaFin);
                            command.Parameters.AddWithValue("@Linea", numLinea);
                            command.Parameters.AddWithValue("@planta", planta);
                            command.Parameters.AddWithValue("@codigo", codigo);
                            command.Parameters.AddWithValue("@corte", corte);
                            command.Parameters.AddWithValue("@filtroDefault", filtroDefault);
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                string JSONstringSp = string.Empty;
                                if (reader.HasRows)
                                {
                                    DataTable table = new DataTable();
                                    table.Load(reader);
                                    using (table)
                                    {
                                        JSONstringSp = JsonConvert.SerializeObject(table);
                                        ReportesDetalle = JsonConvert.DeserializeObject<List<ReportesDetalle>>(JSONstringSp);

                                        return StatusCode(StatusCodes.Status200OK, new { ReportesDetalle });
                                    }
                                }
                                else
                                {
                                    JSONstringSp = JsonConvert.SerializeObject("No se encuentran datos");
                                    return StatusCode(StatusCodes.Status200OK, new { JSONstringSp });
                                }
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

        /// <summary>
        /// Obtenemos el reporte de produccion de acuerdo al filtro seleccionado (ppvc o inyeccion)
        /// </summary>
        /// <param name="turno"></param>
        /// <param name="codigo"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="proceso"></param>
        /// <param name="linea"></param>
        /// <param name="planta"></param>
        /// <param name="corte"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("FilterFinishedProduct")]
        public IActionResult FilterFinishedProduct([FromHeader] string? turno, [FromHeader] string? codigo, [FromHeader] string? fechaInicio, [FromHeader] string? fechaFin, 
            [FromHeader] string proceso, [FromHeader] string? linea, [FromHeader] string planta,
            [FromHeader] string corte)
        {
            try
            {
                int numPlanta = int.Parse(planta);
                string JSONstringSp = string.Empty;
                List<ReportesProdTerm> reportesProdTerm = new List<ReportesProdTerm>();
                //variables creadas para las columnas de scrap
                string fechaInicioScrap = "";
                string fechaFinScrap = "";

                if (corte!= "0" && fechaInicio != null && (fechaFin == null || fechaFin != null))
                {
                    //usamos el corte seleccionado para asignar los horarios y luego reasignamos valor a corte para usar en sql
                    switch (corte.ToString())
                    {
                        case "1": //corte 1 turno 1
                            fechaInicio = $"{fechaInicio} 04:30:01";
                            fechaFin = $"{fechaInicio.Replace("04:30:01", "")} 11:00:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("04:30:01", "")} 05:45:01";
                            fechaFinScrap = $"{fechaFin.Replace("11:00:00", "")} 17:45:00";
                            corte = "1";
                            break;
                        case "2": //corte 2 turno 1
                            fechaInicio = $"{fechaInicio} 11:00:01";
                            fechaFin = $"{fechaInicio.Replace("11:00:01", "")} 16:30:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("11:00:01", "")} 05:45:01";
                            fechaFinScrap = $"{fechaFin.Replace("16:30:00", "")} 17:45:00";
                            corte = "2";
                            break;
                        case "3": //Todo el turno 1
                            fechaInicio = $"{fechaInicio} 04:30:01";
                            fechaFin = $"{fechaInicio.Replace("04:30:01", "")} 16:30:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("04:30:01", "")} 05:45:01";
                            fechaFinScrap = $"{fechaFin.Replace("16:30:00", "")} 17:45:00";
                            corte = "3"; //este 3 se asigna para indicar que es todo el turno en sql
                            break;
                        case "4": //corte 1 turno 2
                            fechaInicio = $"{fechaInicio} 16:30:01";
                            fechaFin = $"{fechaInicio.Replace("16:30:01", "")} 23:00:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("16:30:01", "")} 17:45:01";
                            fechaFinScrap = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 05:45:00";
                            corte = "1";
                            break;
                        case "5": //corte 2 turno 2
                            fechaInicio = $"{fechaInicio} 23:00:01";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("23:00:01", "")} 17:45:01";
                            fechaFinScrap = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 05:45:00";
                            corte = "2";
                            break;
                        case "6": //Todo el turno 2
                            fechaInicio = $"{fechaInicio} 16:30:01";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("16:30:01", "")} 17:45:01";
                            fechaFinScrap = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 05:45:00";
                            corte = "3"; //este 3 se asigna para indicar que es todo el turno en sql
                            break;
                        case "7": //Reporte del dia (turno 1 y 2)
                            fechaInicio = $"{fechaInicio} 04:30:01";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                            fechaInicioScrap = $"{fechaInicio.Replace("04:30:01", "")} 05:45:01";
                            fechaFinScrap = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 05:45:00";
                            corte = null; //asignamos corte como null para que no se envie el turno mas delante
                            break;
                    };
                }

                //Cuando se selecciona un rango de dias
                if (corte == "0" && (fechaInicio != null && fechaFin != null))
                {
                    fechaInicio = $"{fechaInicio} 04:30:01";
                    fechaFin = $"{DateTime.Parse(fechaFin).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00"; //el dia seleccionado como fin, mas 1 dia
                    fechaInicioScrap = $"{fechaInicio.Replace("04:30:01", "")} 05:45:01";
                    fechaFinScrap = $"{fechaFin.Replace("04:30:00", "")} 05:45:00";
                    corte = "4"; //indicamos a sql que se esta seleccionando un rango de dias
                }

                using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                {
                    if (cnn.State == ConnectionState.Closed)
                    {
                        cnn.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = cnn;
                        command.CommandText = "Sppdx_FiltradoProductosTerminados";
                        command.CommandType = CommandType.StoredProcedure;
                        if(codigo != null)
                        {
                            command.Parameters.AddWithValue("@Codigo", codigo);
                        }
                        if(corte != null) //si no enviamos el turno, el procedure asume que es reporte del dia
                        {
                            command.Parameters.AddWithValue("@turno", turno);
                        }
                        command.Parameters.AddWithValue("@proceso", proceso);
                        if(fechaInicio != null)
                        {
                            command.Parameters.AddWithValue("@FechaTurnoInicio", fechaInicio);                            
                        }
                        if(fechaFin != null)
                        {
                            command.Parameters.AddWithValue("@FechaTurnoFin", fechaFin);
                        }
                        if(fechaInicioScrap != null)
                        {
                            command.Parameters.AddWithValue("@FechaTurnoInicioScrap", fechaInicioScrap);
                        }
                        if(fechaFinScrap != null)
                        {
                            command.Parameters.AddWithValue("@FechaTurnoFinScrap", fechaFinScrap);
                        }
                        if(linea != null)
                        {
                            command.Parameters.AddWithValue("@Linea", linea);
                        }
                        command.Parameters.AddWithValue("@Corte", corte);
                        command.Parameters.AddWithValue("@Planta", numPlanta);

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
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
        }

        /// <summary>
        /// EJECUTA EL PROCEDURE PARA DEVOLVER EL SCRAP DE ACUERDO A LA FECHA Y EL CORTE (1 O 2)
        /// </summary>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="proceso"></param>
        /// <param name="corte"></param>
        /// <param name="planta"></param>
        /// <param name="codigo"></param>
        /// <param name="linea"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("FiltroScrapCorte")]
        public IActionResult FiltroScrapCorte([FromHeader] string? fechaInicio, [FromHeader] string? fechaFin,
            [FromHeader] string proceso, [FromHeader] string? corte, [FromHeader] string planta,
            [FromHeader] string? codigo, [FromHeader] string? linea)
        {
            try
            {
                int numPlanta = int.Parse(planta);
                string JSONstringSp = string.Empty;
                List<ReportesScrap> reportesScraps = new List<ReportesScrap>(); 
               
                if (corte != null && (fechaInicio != null && fechaFin != null || fechaFin == null))
                {
                    switch (corte.ToString())
                    {
                        case "1":
                            fechaInicio = $"{fechaInicio} {_configuration["HorariosScrap:HoraInicioCorte1"]}";
                            fechaFin = $"{fechaInicio.Replace(_configuration["HorariosScrap:HoraInicioCorte1"], "")} {_configuration["HorariosScrap:HoraFinCorte1"]}";
                            break;
                        case "2":
                            fechaInicio = $"{fechaInicio} {_configuration["HorariosScrap:HoraInicioCorte2"]}";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} {_configuration["HorariosScrap:HoraFinCorte1"]}";
                            break;
                    };
                }

                using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                {
                    if (cnn.State == ConnectionState.Closed)
                    {
                        cnn.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = cnn;
                        command.CommandText = "Sppdx_FiltradoScrap";
                        command.CommandType = CommandType.StoredProcedure;                       
                        command.Parameters.AddWithValue("@proceso", proceso);
                        if (fechaInicio != null)
                        {
                            command.Parameters.AddWithValue("@FechaTurnoInicio", fechaInicio);
                        }
                        if (fechaFin != null)
                        {
                            command.Parameters.AddWithValue("@FechaTurnoFin", fechaFin);
                        }
                        command.Parameters.AddWithValue("@Planta", numPlanta);
                        command.Parameters.AddWithValue("@Codigo", codigo);
                        command.Parameters.AddWithValue("@Linea", linea);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);                            
                            DataTable tabla = dtDocuments.Tables[0];
                            
                            using (tabla)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla);
                                reportesScraps = JsonConvert.DeserializeObject<List<ReportesScrap>>(JSONstringSp);
                            }
                        }
                        return StatusCode(StatusCodes.Status200OK, new {reportesScraps});

                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }


}
