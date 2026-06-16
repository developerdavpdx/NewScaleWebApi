using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.ScarpRepo
{
    public class ScrapRepositorie : IGenericRepositorieScrap<ScrapMolinos>
    {
        private readonly ApplicationDbContext _applicationDb;

        public ScrapRepositorie(ApplicationDbContext applicationDb)
        {
            _applicationDb = applicationDb;
        }

        public async Task<bool> AddNewScrap(ScrapMolinos entity)
        {
            try
            {
                List<ScrapMolinos> existeRegistro = new List<ScrapMolinos>();
                //Valida que no exista un IdEnvioScrap existente
                if (entity.IdEnvioScrap != null)
                {
                    existeRegistro = _applicationDb.ScrapMolinos.Where(x => x.IdEnvioScrap == entity.IdEnvioScrap).ToList();
                }

                if (existeRegistro.Count() > 0)
                {
                    var registroScrap = _applicationDb.ScrapMolinos.Find(existeRegistro[0].IdScrap);

                    registroScrap.Operador = entity.Operador;
                    registroScrap.CodigoItem = entity.CodigoItem;
                    registroScrap.Peso = entity.Peso;
                    registroScrap.Familia = entity.Familia;
                    registroScrap.Tipo = entity.Tipo;
                    registroScrap.SubFamilia = entity.SubFamilia;
                    registroScrap.Estado = entity.Estado;
                    registroScrap.Turno = entity.Turno;
                    registroScrap.Unidad = entity.Unidad;
                    registroScrap.Proceso = entity.Proceso;
                    registroScrap.FechaScrap = entity.FechaScrap;
                    registroScrap.PlantaScrpt = entity.PlantaScrpt;

                    await _applicationDb.SaveChangesAsync();

                    return true;
                }
                else
                {
                    ScrapMolinos scrap = new ScrapMolinos
                    {
                        Operador = entity.Operador,
                        CodigoItem = entity.CodigoItem,
                        Peso = entity.Peso,
                        Familia = entity.Familia,
                        Tipo = entity.Tipo,
                        SubFamilia = entity.SubFamilia,
                        Estado = entity.Estado,
                        IdEnvioScrap = entity.IdEnvioScrap,
                        Turno = entity.Turno,
                        Proceso = entity.Proceso,
                        Unidad = entity.Unidad,
                        FechaScrap = entity.FechaScrap,
                        AnilloScrpt = 0,
                        AtadoScrpt = 0,
                        Cantidad = 0,
                        FlejeScrpt = 0,
                        TaraScrpt = 0,
                        PlantaScrpt = entity.PlantaScrpt
                    };

                    await _applicationDb.ScrapMolinos.AddAsync(scrap);
                    await _applicationDb.SaveChangesAsync();

                    HistorialPesadas historialPesadas = new HistorialPesadas
                    {
                        Id_Scrap = scrap.IdScrap,
                        Tipo = "Scrap",
                        IdEstatusHP = 1,
                        IdEstatusSAP = 2
                    };

                    await _applicationDb.HistorialPesadas.AddAsync(historialPesadas);
                    await _applicationDb.SaveChangesAsync();

                    return true;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> AddRemolido(ScrapMolinos entity)
        {
            try
            {
                ScrapMolinos scrapMolinos = new ScrapMolinos
                {
                    Operador = entity.Operador,
                    CodigoItem = entity.CodigoItem,
                    Peso = entity.Peso,
                    Familia = entity.Familia,
                    Tipo = entity.Tipo,
                    SubFamilia = entity.SubFamilia,
                    Estado = entity.Estado,
                    Turno = entity.Turno,
                    Proceso = entity.Proceso,
                    Unidad = entity.Unidad,
                    FechaScrap = entity.FechaScrap,
                    IdRemolido = entity.IdRemolido,
                    AnilloScrpt = 0,
                    AtadoScrpt = 0,
                    Cantidad = 0,
                    FlejeScrpt = 0,
                    TaraScrpt = 0,
                    PlantaScrpt = entity.PlantaScrpt
                };

                await _applicationDb.ScrapMolinos.AddAsync(scrapMolinos);
                await _applicationDb.SaveChangesAsync();

                var scrapId = scrapMolinos.IdScrap;

                HistorialPesadas historialPesadas = new HistorialPesadas
                {
                    Id_Scrap = scrapId,
                    Tipo = "Scrap",
                };

                await _applicationDb.HistorialPesadas.AddAsync(historialPesadas);
                await _applicationDb.SaveChangesAsync();

                return true;
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public IList<ScrapMolinos> FilterScrap(string? fechaInicio, string? fechaFin, int? turno, string? estado, string planta)
        {
            IList<ScrapMolinos> scrapMolinos = _applicationDb.ScrapMolinos
                   .Where(x =>
                        (turno == null || x.Turno == turno) &&
                        (estado == null || x.Estado == estado)
                   )
                   .Where(x => (fechaInicio == null && fechaFin == null) || (x.FechaScrap >= DateTime.Parse(fechaInicio) && x.FechaScrap <= DateTime.Parse(fechaFin))
                             || (fechaInicio == null && fechaFin == null && x.FechaScrap >= DateTime.Now.Date.AddDays(-1) && x.FechaScrap <= DateTime.Now.Date.AddDays(1))
                             || (fechaInicio != null && fechaFin == null && x.FechaScrap >= DateTime.Parse(fechaInicio))
                             || (fechaInicio == null && fechaFin != null && x.FechaScrap <= DateTime.Parse(fechaFin))
                    )
                   .Where(x => x.PlantaScrpt == int.Parse(planta))
                   .ToList();


            return scrapMolinos;
        }

        public IList<ScrapMolinos> GetAllScrapMolinos(string planta)
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;

            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                IList<ScrapMolinos> scrapMolinos = _applicationDb.ScrapMolinos
                .Where(x => x.FechaScrap >= horaInicioTurno1 && x.FechaScrap <= horaFinTurno1 && x.PlantaScrpt == int.Parse(planta))
                .Select(x => new ScrapMolinos
                {
                    IdScrap = x.IdScrap,
                    Operador = x.Operador,
                    CodigoItem = x.CodigoItem,
                    Peso = x.Peso,
                    Familia = x.Familia,
                    Tipo = x.Tipo,
                    SubFamilia = x.SubFamilia,
                    Estado = x.Estado,
                    Turno = x.Turno,
                    IdEnvioScrap = x.IdEnvioScrap == null ? null : x.IdEnvioScrap,
                    EnvioScrap = x.EnvioScrap == null ? null : new EnvioScrap
                    {
                        IdEnvioScrap = x.EnvioScrap.IdEnvioScrap,
                        //Tipo = x.EnvioScrap.Tipo,
                        //SubFamilia = x.EnvioScrap.SubFamilia,
                        CodigoItem = x.EnvioScrap.CodigoItem,
                        //Familia = x.EnvioScrap.Familia,
                        Peso = x.EnvioScrap.Peso,
                        Operador = x.EnvioScrap.Operador,
                        Comentarios = x.EnvioScrap.Comentarios
                    },
                    Proceso = x.Proceso,
                    Unidad = x.Unidad,
                    FechaScrap = x.FechaScrap,
                    AnilloScrpt = x.AnilloScrpt,
                    AtadoScrpt = x.AtadoScrpt,
                    Cantidad = x.Cantidad,
                    FlejeScrpt = x.FlejeScrpt,
                    TaraScrpt = x.TaraScrpt
                }).ToList();

                return scrapMolinos;
            }
            else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                IList<ScrapMolinos> scrapMolinos = _applicationDb.ScrapMolinos
                    .Where(x => x.FechaScrap >= horaInicioTurno2 && x.FechaScrap <= horaFinTurno2)
                    .Select(x => new ScrapMolinos
                    {
                        IdScrap = x.IdScrap,
                        Operador = x.Operador,
                        CodigoItem = x.CodigoItem,
                        Peso = x.Peso,
                        Familia = x.Familia,
                        Tipo = x.Tipo,
                        SubFamilia = x.SubFamilia,
                        Estado = x.Estado,
                        Turno = x.Turno,
                        IdEnvioScrap = x.IdEnvioScrap == null ? null : x.IdEnvioScrap,
                        EnvioScrap = x.EnvioScrap == null ? null : new EnvioScrap
                        {
                            IdEnvioScrap = x.EnvioScrap.IdEnvioScrap,
                            //Tipo = x.EnvioScrap.Tipo,
                            //SubFamilia = x.EnvioScrap.SubFamilia,
                            CodigoItem = x.EnvioScrap.CodigoItem,
                            //Familia = x.EnvioScrap.Familia,
                            Peso = x.EnvioScrap.Peso,
                            Operador = x.EnvioScrap.Operador,
                            Comentarios = x.EnvioScrap.Comentarios
                        },
                        Proceso = x.Proceso,
                        Unidad = x.Unidad,
                        FechaScrap = x.FechaScrap,
                        AnilloScrpt = x.AnilloScrpt,
                        AtadoScrpt = x.AtadoScrpt,
                        Cantidad = x.Cantidad,
                        FlejeScrpt = x.FlejeScrpt,
                        TaraScrpt = x.TaraScrpt
                    }).ToList();

                return scrapMolinos;
            }
        }

        public int GetNextIdScrap()
        {
            ScrapMolinos registroScrap = _applicationDb.ScrapMolinos.OrderByDescending(x => x.IdScrap).FirstOrDefault();
            if (registroScrap == null)
                return 0;

            int id = registroScrap.IdScrap;
            return id;
        }

        public IEnumerable<object> GetSumLineas(int turno, DateTime horaTurnoStart, DateTime horaTurnoEnd)
        {
            DateTime horaInicioTurno1 = horaTurnoStart;
            DateTime horaFinTurno1 = horaTurnoEnd;
            DateTime horaInicioTurno2 = horaTurnoStart;
            DateTime horaFinTurno2 = horaTurnoEnd;

            DateTime horaActual = DateTime.Now;

            //si el turno es 1
            if (turno == 1)
            {
                IEnumerable<object> totalScrapLinea = from sm in _applicationDb.ScrapMolinos
                                                      join es in _applicationDb.EnvioScraps on sm.IdEnvioScrap equals es.IdEnvioScrap
                                                      where sm.IdEnvioScrap != null && (sm.FechaScrap >= horaInicioTurno1 && sm.FechaScrap <= horaFinTurno1)
                                                      group sm by es.Linea into g
                                                      orderby g.Key ascending
                                                      select new
                                                      {
                                                          Linea = g.Key,
                                                          Peso = (g.Sum(x => x.Peso))
                                                      };


                return totalScrapLinea;
            }
            //si el turno es 2
            else
            {   
                //si estamos en el rango del turno 2 durante la madrugada
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }
                IEnumerable<object> totalScrapLinea = from sm in _applicationDb.ScrapMolinos
                                                      join es in _applicationDb.EnvioScraps on sm.IdEnvioScrap equals es.IdEnvioScrap
                                                      where sm.IdEnvioScrap != null && (sm.FechaScrap >= horaInicioTurno2 && sm.FechaScrap <= horaFinTurno2)
                                                      group sm by es.Linea into g
                                                      orderby g.Key ascending
                                                      select new
                                                      {
                                                          Linea = g.Key,
                                                          Peso = (g.Sum(x => x.Peso))
                                                      };


                return totalScrapLinea;
            }
        }
    }
}
