using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPtmHana.BLL.Services.EnvioMolinosServ;
using WebApiPtmHana.BLL.Services.LineasServ;
using WebApiPtmHana.BLL.Services.ScrapServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models.EnvioMolinos;
using WebApiPtmHana.Models.HistorialPesadasModel;
using WebApiPtmHana.Models.Reportes;
using WebApiPtmHana.Models.Scrap;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScrapController : ControllerBase
    {
        private readonly IScrap _scrapService;
        private readonly IEnvioMolinos _envioMolinos;
        private readonly ApplicationDbContext _applicationDb;
        private readonly IWebHostEnvironment _env;
        private readonly ILinia _lineas;

        public ScrapController(IScrap scrapService, IEnvioMolinos envioMolinos, ApplicationDbContext applicationDb,
            IWebHostEnvironment env, ILinia lineas)
        {
            _scrapService = scrapService;
            _envioMolinos = envioMolinos;
            _applicationDb = applicationDb;
            _env = env;
            _lineas = lineas;
        }

        [HttpPost]
        [Route("SendToMills")]
        public async Task<IActionResult> SendToMills([FromBody] EnvioMolinosModel molinos)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Errores dentro del envió del modelo" });

                // Consulta para obtener CodigoLinea basado en la Linea
                var CodigoLinea = _lineas.GetCode(molinos.planta.ToString(), molinos.Linea.ToString());

                if (CodigoLinea == null) { 
                
                }


                var molinoScrap = new EnvioScrap()
                {
                    CodigoItem = molinos.CodigoItemEM,
                    Comentarios = molinos.ComentariosEM,
                    Familia = molinos.FamiliaEM,
                    Operador = molinos.OperadorEM,
                    Peso = molinos.PesoEM,
                    //SubFamilia = molinos.SubFamiliaEM,
                    //Tipo = molinos.TipoEM,
                    Turno = molinos.TurnoEM,
                    Proceso = molinos.ProcesoEM,
                    Unidad = molinos.UnidadEM,
                    FechaEnvio = DateTime.Now,
                    Linea = molinos.Linea,
                    ScrapPlanta = molinos.planta,
                    CodigoLinea = CodigoLinea.ItemCodeLinea
                };

                string result = await _envioMolinos.SendToMills(molinoScrap);

                if (result != "true")
                    return StatusCode(StatusCodes.Status400BadRequest, new { response = result });

                return StatusCode(StatusCodes.Status200OK, new { response = "Se a enviado a molinos correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAfterIdProduction")]
        public IActionResult GetAfterIdProduction([FromHeader] string idProduccion)
        {
            try
            {
                if (idProduccion == null)
                    return StatusCode(StatusCodes.Status400BadRequest, new { response = "El id no puede estar vacío" });

                var result = _envioMolinos.GetEnvioScrapById(int.Parse(idProduccion));

                if (result.Count() == 0)
                    return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentra información de envio Scrap" });

                return StatusCode(StatusCodes.Status200OK, new { result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("AddScrap")]
        public async Task<IActionResult> AddScrap([FromBody] Scrap scrap)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Errores dentro del envió del modelo" });

                ScrapMolinos scrap1 = new ScrapMolinos()
                {
                    Turno = scrap.TurnoScrap,
                    CodigoItem = scrap.CodigoItemScrap,
                    Estado = scrap.EstadoScrap,
                    Familia = scrap.FamiliaScrap,
                    IdEnvioScrap = scrap.IdEnvioScrap,
                    Operador = scrap.OperadorScrap,
                    Peso = scrap.PesoScrap,
                    SubFamilia = scrap.SubFamiliaScrap,
                    Tipo = scrap.TipoScrap,
                    Proceso = scrap.ProcesoScrap,
                    Unidad = scrap.UnidadScrap,
                    FechaScrap = DateTime.Now,
                    PlantaScrpt = scrap.planta
                };

                bool result = await _scrapService.AddNewScrap(scrap1);

                if (!result)
                    return StatusCode(StatusCodes.Status400BadRequest, new { response = "No se ha podido agregar el Scrap" });

                return StatusCode(StatusCodes.Status200OK, new { response = "Scrap agregado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAllReciboScrap")]
        public ActionResult GetAllReciboScrap([FromHeader] string planta)
        {
            try
            {
                var response = _envioMolinos.GetEnvioScrap(planta);

                if (response.Count >= 0)
                    return StatusCode(StatusCodes.Status200OK, new { response });

                return StatusCode(StatusCodes.Status400BadRequest, new { response = "No se han obtenido los registros" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAllScrap")]
        public async Task<IActionResult> GetAllScrap(string planta)
        {
            int numplanta = int.Parse(planta);
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;

            DateTime primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime ultimiDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);

            try
            {
                if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
                {
                    IList<ReporteInventarioFisicoscrap> result = (from s in _applicationDb.ScrapMolinos
                                                                  where s.Tipo == "Bueno" || s.Tipo == "Quemado/Contaminado" || s.Tipo == "Baja" && s.FechaScrap >= primerDiaMes && s.FechaScrap <= ultimiDiaMes && s.PlantaScrpt == numplanta
                                                                  group s by new { s.Proceso, s.Familia, s.SubFamilia, s.Estado, s.Unidad, s.Tipo } into g
                                                                  select new ReporteInventarioFisicoscrap
                                                                  {
                                                                      Proceso = g.Key.Proceso,
                                                                      Familia = g.Key.Familia,
                                                                      SubFamilia = g.Key.SubFamilia,
                                                                      Estado = g.Key.Estado,
                                                                      Unidad = g.Key.Unidad,
                                                                      Tipo = g.Key.Tipo,
                                                                      Pesot = g.Sum(x => x.Peso),
                                                                      indexSubFamilia = (g.Key.SubFamilia == "Scrap" ? 0 :
                                                                      g.Key.SubFamilia == "Viruta" ? 1 :
                                                                      g.Key.SubFamilia == "Molido" ? 2 :
                                                                      g.Key.SubFamilia == "Pulverizado" ? 3 :
                                                                      g.Key.SubFamilia == "Polvo de Fltros de Vacio" ? 4 :
                                                                      g.Key.SubFamilia == "Compuesto Virgen en Saco" ? 5 : null

                                                                      )
                                                                  }).ToList();

                    var horaInicioAyer = horaInicioTurno1.AddDays(-1);
                    var horaFinAyer = horaFinTurno2.AddDays(-1);

                    IList<ReporteInventarioFisicoscrap> registrosAyer = (from s in _applicationDb.ScrapMolinos
                                                                         where s.FechaScrap >= horaInicioAyer && s.FechaScrap <= horaFinAyer && s.Proceso == "PPVC" && s.PlantaScrpt == numplanta
                                                                         group s by new { s.Proceso, s.Familia, s.SubFamilia, s.Estado, s.Unidad, s.Tipo } into g
                                                                         select new ReporteInventarioFisicoscrap
                                                                         {
                                                                             Proceso = g.Key.Proceso,
                                                                             Familia = g.Key.Familia,
                                                                             SubFamilia = g.Key.SubFamilia,
                                                                             Estado = g.Key.Estado,
                                                                             Unidad = g.Key.Unidad,
                                                                             Tipo = g.Key.Tipo,
                                                                             Pesot = g.Sum(x => x.Peso),
                                                                             indexSubFamilia = (g.Key.SubFamilia == "Scrap" ? 0 :
                                                                             g.Key.SubFamilia == "Viruta" ? 1 :
                                                                             g.Key.SubFamilia == "Molido" ? 2 :
                                                                             g.Key.SubFamilia == "Pulverizado" ? 3 :
                                                                             g.Key.SubFamilia == "Polvo de Fltros de Vacio" ? 4 :
                                                                             g.Key.SubFamilia == "Compuesto Virgen en Saco" ? 5 : null

                                                                             )
                                                                         }).ToList();

                    IList<ReporteInventarioFisicoscrap> registrosHoy = (from s in _applicationDb.ScrapMolinos
                                                                        where s.Tipo == "Bueno" || s.Tipo == "Quemado/Contaminado" || s.Tipo == "Baja" && s.FechaScrap >= horaInicioTurno1 && s.FechaScrap < horaFinTurno2 && s.Proceso == "PPVC" && s.PlantaScrpt == numplanta
                                                                        group s by new { s.Proceso, s.Familia, s.SubFamilia, s.Estado, s.Unidad, s.Tipo } into g
                                                                        select new ReporteInventarioFisicoscrap
                                                                        {
                                                                            Proceso = g.Key.Proceso,
                                                                            Familia = g.Key.Familia,
                                                                            SubFamilia = g.Key.SubFamilia,
                                                                            Estado = g.Key.Estado,
                                                                            Unidad = g.Key.Unidad,
                                                                            Tipo = g.Key.Tipo,
                                                                            Pesot = g.Sum(x => x.Peso),
                                                                            indexSubFamilia = (g.Key.SubFamilia == "Scrap" ? 0 :
                                                                            g.Key.SubFamilia == "Viruta" ? 1 :
                                                                            g.Key.SubFamilia == "Molido" ? 2 :
                                                                            g.Key.SubFamilia == "Pulverizado" ? 3 :
                                                                            g.Key.SubFamilia == "Polvo de Fltros de Vacio" ? 4 :
                                                                            g.Key.SubFamilia == "Compuesto Virgen en Saco" ? 5 : null

                                                                            )
                                                                        }).ToList();

                    var AcumuladoEmbarque = (from x in _applicationDb.AcumuladoEmbarques
                                             where x.FechaIngreso >= horaInicioTurno1 && x.FechaIngreso <= horaFinTurno1 && x.Planta == numplanta
                                             group x by new { x.Proceso, x.Familia } into g
                                             select new
                                             {
                                                 Proceso = g.Key.Proceso,
                                                 Familia = g.Key.Familia,
                                                 Sumatoria = g.Sum(x => x.Acumulado)
                                             });

                    return StatusCode(StatusCodes.Status200OK, new { result, AcumuladoEmbarque, registrosHoy, registrosAyer });
                }
                else
                {
                    var horaInicioAyer = horaInicioTurno1.AddDays(-1);
                    var horaFinAyer = horaFinTurno2.AddDays(-1);
                    if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                    {
                        horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                        horaFinTurno2 = horaFinTurno2.AddDays(-1);
                    }

                    IList<ReporteInventarioFisicoscrap> result = (from s in _applicationDb.ScrapMolinos
                                                                  where s.Tipo == "Bueno" || s.Tipo == "Quemado/Contaminado" || s.Tipo == "Baja" && s.FechaScrap >= primerDiaMes && s.FechaScrap <= ultimiDiaMes && s.PlantaScrpt == numplanta
                                                                  group s by new { s.Proceso, s.Familia, s.SubFamilia, s.Estado, s.Unidad, s.Tipo } into g
                                                                  select new ReporteInventarioFisicoscrap
                                                                  {
                                                                      Proceso = g.Key.Proceso,
                                                                      Familia = g.Key.Familia,
                                                                      SubFamilia = g.Key.SubFamilia,
                                                                      Estado = g.Key.Estado,
                                                                      Unidad = g.Key.Unidad,
                                                                      Tipo = g.Key.Tipo,
                                                                      Pesot = g.Sum(x => x.Peso),
                                                                      indexSubFamilia = (g.Key.SubFamilia == "Scrap" ? 0 :
                                                                      g.Key.SubFamilia == "Viruta" ? 1 :
                                                                      g.Key.SubFamilia == "Molido" ? 2 :
                                                                      g.Key.SubFamilia == "Pulverizado" ? 3 :
                                                                      g.Key.SubFamilia == "Polvo de Fltros de Vacio" ? 4 :
                                                                      g.Key.SubFamilia == "Compuesto Virgen en Saco" ? 5 : null

                                                                      )
                                                                  }).ToList();

                    IList<ReporteInventarioFisicoscrap> registrosAyer = (from s in _applicationDb.ScrapMolinos
                                                                         where s.Tipo == "Bueno" || s.Tipo == "Quemado/Contaminado" || s.Tipo == "Baja" && s.FechaScrap >= horaInicioAyer && s.FechaScrap <= horaFinAyer && s.PlantaScrpt == numplanta
                                                                         group s by new { s.Proceso, s.Familia, s.SubFamilia, s.Estado, s.Unidad, s.Tipo } into g
                                                                         select new ReporteInventarioFisicoscrap
                                                                         {
                                                                             Proceso = g.Key.Proceso,
                                                                             Familia = g.Key.Familia,
                                                                             SubFamilia = g.Key.SubFamilia,
                                                                             Estado = g.Key.Estado,
                                                                             Unidad = g.Key.Unidad,
                                                                             Tipo = g.Key.Tipo,
                                                                             Pesot = g.Sum(x => x.Peso),
                                                                             indexSubFamilia = (g.Key.SubFamilia == "Scrap" ? 0 :
                                                                             g.Key.SubFamilia == "Viruta" ? 1 :
                                                                             g.Key.SubFamilia == "Molido" ? 2 :
                                                                             g.Key.SubFamilia == "Pulverizado" ? 3 :
                                                                             g.Key.SubFamilia == "Polvo de Fltros de Vacio" ? 4 :
                                                                             g.Key.SubFamilia == "Compuesto Virgen en Saco" ? 5 : null

                                                                             )
                                                                         }).ToList();

                    IList<ReporteInventarioFisicoscrap> registrosHoy = (from s in _applicationDb.ScrapMolinos
                                                                        where s.Tipo == "Bueno" || s.Tipo == "Quemado/Contaminado" || s.Tipo == "Baja" && s.FechaScrap >= horaInicioTurno1 && s.FechaScrap <= horaFinTurno2 && s.PlantaScrpt == numplanta
                                                                        group s by new { s.Proceso, s.Familia, s.SubFamilia, s.Estado, s.Unidad, s.Tipo } into g
                                                                        select new ReporteInventarioFisicoscrap
                                                                        {
                                                                            Proceso = g.Key.Proceso,
                                                                            Familia = g.Key.Familia,
                                                                            SubFamilia = g.Key.SubFamilia,
                                                                            Estado = g.Key.Estado,
                                                                            Unidad = g.Key.Unidad,
                                                                            Tipo = g.Key.Tipo,
                                                                            Pesot = g.Sum(x => x.Peso),
                                                                            indexSubFamilia = (g.Key.SubFamilia == "Scrap" ? 0 :
                                                                            g.Key.SubFamilia == "Viruta" ? 1 :
                                                                            g.Key.SubFamilia == "Molido" ? 2 :
                                                                            g.Key.SubFamilia == "Pulverizado" ? 3 :
                                                                            g.Key.SubFamilia == "Polvo de Fltros de Vacio" ? 4 :
                                                                            g.Key.SubFamilia == "Compuesto Virgen en Saco" ? 5 : null

                                                                            )
                                                                        }).ToList();

                    var AcumuladoEmbarque = (from x in _applicationDb.AcumuladoEmbarques
                                             where x.FechaIngreso >= horaInicioTurno2 && x.FechaIngreso <= horaFinTurno2 && x.Planta == numplanta
                                             group x by new { x.Proceso, x.Familia } into g
                                             select new
                                             {
                                                 Proceso = g.Key.Proceso,
                                                 Familia = g.Key.Familia,
                                                 Sumatoria = g.Sum(x => x.Acumulado)
                                             });

                    return StatusCode(StatusCodes.Status200OK, new { result, AcumuladoEmbarque, registrosHoy, registrosAyer });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("AddRemolido")]
        public async Task<IActionResult> AddRemolido([FromBody] Scrap scrap)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Errores dentro del envió del modelo" });

            ScrapMolinos scrap1 = new ScrapMolinos()
            {
                Turno = scrap.TurnoScrap,
                CodigoItem = scrap.CodigoItemScrap,
                Estado = scrap.EstadoScrap,
                Familia = scrap.FamiliaScrap,
                Operador = scrap.OperadorScrap,
                Peso = scrap.PesoScrap,
                SubFamilia = scrap.SubFamiliaScrap,
                Tipo = scrap.TipoScrap,
                Proceso = scrap.ProcesoScrap,
                Unidad = scrap.UnidadScrap,
                FechaScrap = DateTime.Now,
                IdRemolido = scrap.IdRemolidoScrap,
                PlantaScrpt = scrap.planta
            };

            bool result = await _scrapService.AddRemolidoScrap(scrap1);

            if (!result)
                return StatusCode(StatusCodes.Status400BadRequest, new { response = "No se ha podido agregar el Scrap" });

            return StatusCode(StatusCodes.Status200OK, new { response = "Scrap agregado correctamente" });
        }

        [HttpGet]
        [Route("GetNextIdEnvioScrap")]
        public IActionResult GetNextIdEnvioScrap()
        {
            try
            {
                int id = _envioMolinos.GetNextIdEnvioMolinos();
                return StatusCode(StatusCodes.Status200OK, new { response = id }); ;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetNextIdScrap")]
        public IActionResult GetNextIdScrap()
        {
            try
            {
                int id = _scrapService.GetNextIdScrap();
                return StatusCode(StatusCodes.Status200OK, new { response = id }); ;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("FilterScrap")]
        public IActionResult FliterScrap([FromHeader] string? fechaInicio, [FromHeader] string? fechaFin, [FromHeader] string? turno, [FromHeader] string planta)
        {
            try
            {
                switch (turno!.ToString())
                {
                    case "1":
                        fechaInicio = $"{fechaInicio} 04:30:00";
                        fechaFin = $"{fechaFin} 16:30:00";
                        break;
                    case "2":
                        fechaInicio = $"{fechaInicio} 16:30:00";
                        fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                        break;
                    case "3":
                        fechaInicio = $"{fechaInicio} 04:30:00";
                        fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                        turno = null;
                        break;
                };



                var scrapFiltrado = _envioMolinos.FilterReciboScrap(fechaInicio, fechaFin, turno != null ? int.Parse(turno) : null, planta);
                if (scrapFiltrado.Count >= 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { scrapFiltrado });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No se ha podido filtrar los resultados" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("getPlantillaExcel")]
        public IActionResult getPlantillaExcel()
        {
            try
            {
                string path1 = _env.ContentRootPath;
                string filePath = path1 + "/TicketPTM/datos.xlsx";
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(fileBytes);

                // crear un objeto de respuesta con la cadena Base64
                var response = new
                {
                    archivoBase64 = base64
                };

                Console.WriteLine(response);
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("EditScrap")]
        public async Task<IActionResult> EditScrap([FromBody] EditarHitorialPesadas editScrpt)
        {
            try
            {
                var scrap = _applicationDb.HistorialPesadas.Find(editScrpt.idHistPesada);

                if(scrap != null)
                {
                    var listScrap = await _applicationDb.ScrapMolinos.FindAsync(scrap.Id_Scrap);

                    if(listScrap != null)
                    {
                        //if (editScrpt.LineaPtH != null)
                        //    listScrap.

                        if (editScrpt.PesoTotalPtH != null)
                            listScrap.Peso = float.Parse(editScrpt.PesoTotalPtH.ToString());

                        if (editScrpt.NumTubosPtH != null)
                            listScrap.Cantidad = int.Parse(editScrpt.NumTubosPtH.ToString());

                        if (editScrpt.AtadoPTH != null)
                            listScrap.AtadoScrpt = float.Parse(editScrpt.AtadoPTH.ToString());

                        if (editScrpt.TaraPTH != null)
                            listScrap.TaraScrpt = float.Parse(editScrpt.TaraPTH.ToString());

                        if (editScrpt.AnilloPTH != null)
                            listScrap.AnilloScrpt = float.Parse(editScrpt.AnilloPTH.ToString());

                        if (editScrpt.FlejePTH != null)
                            listScrap.FlejeScrpt = float.Parse(editScrpt.FlejePTH.ToString());

                        _applicationDb.Update(listScrap);
                        await _applicationDb.SaveChangesAsync();

                        return StatusCode(StatusCodes.Status200OK, new { response = "Datos actualizados correctamente" });
                    } else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existen datos" });
                    }
                } else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existen datos" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetScrap")]
        public IActionResult GetScrapById([FromHeader] string id)
        {
            try
            {
                int IdScrap = int.Parse(id);
                List<ScrapMolinos> scrapInfo = _applicationDb.ScrapMolinos.Where(x => x.IdScrap == IdScrap).ToList();

                if (scrapInfo.Count > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = scrapInfo });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe registro de Scrap" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }
}
