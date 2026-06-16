using System.Diagnostics;
using System.Numerics;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.HistorialPesadasRepo
{
    public class HistorialPesadasRepositorie : IGenericRepoHistorialPesadas<HistorialPesadas>
    {
        private readonly ApplicationDbContext _context;

        public HistorialPesadasRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddComent(int idHistPesada, string comentario)
        {
            HistorialPesadas historialPesadas = await _context.HistorialPesadas.FindAsync(idHistPesada);
            if (historialPesadas == null)
                return false;

            historialPesadas.Comentario = comentario;

            try
            {
                _context.Update(historialPesadas);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool CopyRegister(int idHPesada, string comentario, string Codigo, int Id_Linea,
            float PesoTotal, int NumTubos, float AtadoPT, float TaraPT, float AnilloPT, float FlejePT)
        {


            //historialPesadas.Comentario = comentario;
            //historialPesadas.IdEstatusHP = 2;
            //_context.Update(historialPesadas);

            //agregamos el registro duplicado en la tabla de PT //<--
            var historialPesadas = _context.HistorialPesadas.Find(idHPesada);
            
            var productoTerminado = _context.ProductoTerminados.Find(historialPesadas.Id_ProductoTerminado);
            ProductoTerminado productoTerminado1 = new ProductoTerminado()
            {
                Codigo = Codigo, //<<
                CodigoNombre = productoTerminado.CodigoNombre,
                Clasification = productoTerminado.Clasification,
                Id_Linea = Id_Linea,//<<
                FechaPesaje = productoTerminado.FechaPesaje,
                PesoTotal = PesoTotal,//<<
                NumTubos = NumTubos, //<<
                TurnoPT = productoTerminado.TurnoPT,
                AtadoPT = AtadoPT, //<<
                TaraPT = TaraPT, //<<
                AnilloPT = AnilloPT, //<<
                FlejePT = FlejePT, //<<
                FolioPT = productoTerminado.FolioPT,
                KgxHrbyPt = productoTerminado.KgxHrbyPt,
                Calidad = productoTerminado.Calidad,
                Proceso = productoTerminado.Proceso,
                OrdenFabricacion = productoTerminado.OrdenFabricacion,
                Planta = productoTerminado.Planta,
                Desactivado = productoTerminado.Desactivado
            };
            _context.Add(productoTerminado1);
            _context.SaveChanges(); //<--



            //Crear nuevo registro de HISTORIAL PESADAS a partir de la copia
            HistorialPesadas historial = new HistorialPesadas()
            {
                Id_Scrap = historialPesadas.Id_Scrap == null ? null : historialPesadas.Id_Scrap,
                Id_ProductoTerminado = productoTerminado1.Id,
                Tipo = historialPesadas.Tipo == "PT" ? "PT" : "Scrap",
                Comentario = comentario,
                IdEstatusHP = 1,
                IdEstatusSAP = 2
            };

            _context.Add(historial);
            _context.SaveChanges();

            //Colocar registro original como estatus duplicado
            historialPesadas.IdEstatusHP = 2;
            _context.Update(historialPesadas);

            return true;
        }

        public async Task<bool> DisableRegister(int idHistPesada, string comentario)
        {
            HistorialPesadas historialPesadas = await _context.HistorialPesadas.FindAsync(idHistPesada);
            if (historialPesadas == null)
                return false;
            historialPesadas.IdEstatusHP = 4;
            historialPesadas.Comentario = comentario;
            ProductoTerminado productoTerminado = await _context.ProductoTerminados.FindAsync(historialPesadas.Id_ProductoTerminado);
            productoTerminado.Desactivado = 1; //1 desactivado , 0 activado
            _context.Update(historialPesadas);
            _context.Update(productoTerminado);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditRecordById(int idHistPesada, int? LineaPtH, float? PesoTotalPtH, int? NumTubosPtH, float? AtadoPTH, float? TaraPTH, float? AnilloPTH, float? FlejePTH, string comentario, string nombreTrabjador, int idSKU)
        {
            HistorialPesadas historial = await _context.HistorialPesadas.FindAsync(idHistPesada);

            if (historial == null)
                return false;

            historial.Comentario = comentario;
            historial.IdEstatusHP = 3;

            _context.Update(historial);
            ProductoTerminado productoTerminado = await _context.ProductoTerminados.FindAsync(historial.Id_ProductoTerminado);

            BitHistorialPesadas historicalProductoTerm = new BitHistorialPesadas
            {
                Codigo = productoTerminado.Codigo,
                CodigoNombre = productoTerminado.CodigoNombre,
                LineaPtH = LineaPtH == productoTerminado!.Id_Linea ? null : productoTerminado!.Id_Linea,
                IdHistorialPesada = idHistPesada,
                PesoTotalPtH = PesoTotalPtH == productoTerminado!.PesoTotal ? null : productoTerminado!.PesoTotal,
                AnilloPTH = AnilloPTH == productoTerminado!.AnilloPT ? null : productoTerminado!.AnilloPT,
                AtadoPTH = AtadoPTH == productoTerminado!.AtadoPT ? null : productoTerminado!.AtadoPT,
                TaraPTH = TaraPTH == productoTerminado!.TaraPT ? null : productoTerminado!.TaraPT,
                FlejePTH = FlejePTH == productoTerminado!.FlejePT ? null : productoTerminado!.FlejePT,
                NumTubosPtH = NumTubosPtH == productoTerminado!.NumTubos ? null : productoTerminado!.NumTubos,
                Nombre = nombreTrabjador,
                FechaModificacion = DateTime.Now
            };

            await _context.BitHistorialPesadas.AddAsync(historicalProductoTerm);

            if (LineaPtH != productoTerminado!.Id_Linea)
                productoTerminado!.Id_Linea = LineaPtH;

            if (PesoTotalPtH != productoTerminado!.PesoTotal)
                productoTerminado!.PesoTotal = PesoTotalPtH;

            if (NumTubosPtH != productoTerminado!.NumTubos)
                productoTerminado!.NumTubos = NumTubosPtH;

            if (AtadoPTH != productoTerminado!.AtadoPT)
                productoTerminado!.AtadoPT = AtadoPTH;

            if (TaraPTH != productoTerminado!.TaraPT)
                productoTerminado!.TaraPT = TaraPTH;

            if (AnilloPTH != productoTerminado!.AnilloPT)
                productoTerminado!.AnilloPT = AnilloPTH;

            if (FlejePTH != productoTerminado!.FlejePT)
                productoTerminado!.FlejePT = FlejePTH;

            _context.Update(productoTerminado!);

            await _context.SaveChangesAsync();

            return true;
        }

        public IList<HistorialPesadas> FilterInfoHistorical(int? turno, string? codigo, int? idEstadoSap, string? fechaInicio, string? fechaFin, int planta, int? linea)
        {
            try
            {
                var historialPesadas = _context.HistorialPesadas
                .Where(x => (x.ProductoTerminado != null && (x.ProductoTerminado.Planta == planta)))
                .Where(x => turno == null || (x.ProductoTerminado != null && (x.ProductoTerminado.TurnoPT ?? 0) == turno) || (x.ScrapMolinos != null && x.ScrapMolinos.Turno == turno))
                .Where(x => idEstadoSap == null || idEstadoSap == x.IdEstatusSAP)
                .Where(x => linea == null || (x.ProductoTerminado != null && (x.ProductoTerminado.Id_Linea ?? 0) == linea))
                .Where(x => codigo == null || (x.ScrapMolinos != null && x.ScrapMolinos.CodigoItem == codigo) || (x.ProductoTerminado != null && x.ProductoTerminado.Codigo == codigo))
                .Where(x => (fechaInicio == null && fechaFin == null) ||
                    (x.ProductoTerminado != null && x.ProductoTerminado.FechaPesaje >= DateTime.Parse(fechaInicio) && x.ProductoTerminado.FechaPesaje <= DateTime.Parse(fechaFin)) ||
                    (x.ScrapMolinos != null && x.ScrapMolinos.FechaScrap >= DateTime.Parse(fechaInicio) && x.ScrapMolinos.FechaScrap <= DateTime.Parse(fechaFin))
                )
                .Select(x => new HistorialPesadas
                {
                    Id = x.Id,
                    Id_Scrap = x.Id_Scrap,
                    IdEstatusHP = x.IdEstatusHP,
                    IdEstatusSAP = x.IdEstatusSAP,
                    DocNum = x.DocNum,
                    EstadosSAP = new EstadosSAP

                    {
                        Id = x.EstadosSAP.Id,
                        Descripcion = x.EstadosSAP.Descripcion,
                        Estatus = x.EstadosSAP.Estatus
                    },
                    ScrapMolinos = x.ScrapMolinos == null ? null : new ScrapMolinos
                    {
                        Operador = x.ScrapMolinos.Operador,
                        CodigoItem = x.ScrapMolinos.CodigoItem,
                        Peso = float.Parse(x.ScrapMolinos.Peso.ToString()),
                        Familia = x.ScrapMolinos.Familia,
                        Tipo = x.ScrapMolinos.Tipo,
                        SubFamilia = x.ScrapMolinos.SubFamilia,
                        Estado = x.ScrapMolinos.Estado,
                        IdEnvioScrap = x.ScrapMolinos.IdEnvioScrap,
                        EnvioScrap = x.ScrapMolinos.EnvioScrap == null ? null : new EnvioScrap
                        {
                            IdEnvioScrap = x.ScrapMolinos.EnvioScrap.IdEnvioScrap,
                            CodigoItem = x.ScrapMolinos.EnvioScrap.CodigoItem,
                            //Familia = x.ScrapMolinos.EnvioScrap.Familia,
                            Comentarios = x.ScrapMolinos.EnvioScrap.Comentarios,
                            FechaEnvio = x.ScrapMolinos.EnvioScrap.FechaEnvio,
                            Operador = x.ScrapMolinos.EnvioScrap.Operador,
                            Peso = x.ScrapMolinos.EnvioScrap.Peso,
                            Proceso = x.ScrapMolinos.EnvioScrap.Proceso,
                            //SubFamilia = x.ScrapMolinos.EnvioScrap.SubFamilia,
                            //Tipo = x.ScrapMolinos.EnvioScrap.Tipo,
                            Turno = x.ScrapMolinos.EnvioScrap.Turno,
                            Unidad = x.ScrapMolinos.EnvioScrap.Unidad,
                            Linea = x.ScrapMolinos.EnvioScrap.Linea
                        },
                        Turno = x.ScrapMolinos.Turno,
                        Proceso = x.ScrapMolinos.Proceso,
                        Unidad = x.ScrapMolinos.Unidad,
                        FechaScrap = x.ScrapMolinos.FechaScrap,
                        IdScrap = x.ScrapMolinos.IdScrap,
                        AnilloScrpt = float.Parse(x.ScrapMolinos.AnilloScrpt.ToString()),
                        AtadoScrpt = float.Parse(x.ScrapMolinos.AtadoScrpt.ToString()),
                        TaraScrpt = float.Parse(x.ScrapMolinos.TaraScrpt.ToString()),
                        FlejeScrpt = float.Parse(x.ScrapMolinos.FlejeScrpt.ToString()),
                        Cantidad = x.ScrapMolinos.Cantidad
                    },
                    Id_ProductoTerminado = x.Id_ProductoTerminado,
                    ProductoTerminado = x.ProductoTerminado == null ? null : new ProductoTerminado
                    {
                        Id = x.ProductoTerminado.Id,
                        Clasification = x.ProductoTerminado.Clasification,
                        KgxHrbyPt = x.ProductoTerminado.KgxHrbyPt,
                        Codigo = x.ProductoTerminado.Codigo,
                        CodigoNombre = x.ProductoTerminado.CodigoNombre,
                        Calidad = x.ProductoTerminado.Calidad,
                        Proceso = x.ProductoTerminado.Proceso,
                        Id_Linea = x.ProductoTerminado.Id_Linea,
                        Lineas = x.ProductoTerminado.Id_Linea == null ? null : new Lineas
                        {
                            Id_linea = x.ProductoTerminado.Lineas.Id_linea,
                            Asignado = x.ProductoTerminado.Lineas.Asignado,
                            Descripcion = x.ProductoTerminado.Lineas.Descripcion,
                            Proceso = x.ProductoTerminado.Lineas.Proceso
                        },
                        FechaPesaje = x.ProductoTerminado.FechaPesaje,
                        NumTubos = x.ProductoTerminado.NumTubos,
                        PesoTotal = (float)x.ProductoTerminado.PesoTotal,
                        AnilloPT = (float)x.ProductoTerminado.AnilloPT,
                        AtadoPT = (float)x.ProductoTerminado.AtadoPT,
                        FlejePT = (float)x.ProductoTerminado.FlejePT,
                        FolioPT = x.ProductoTerminado.FolioPT,
                        TaraPT = (float)x.ProductoTerminado.TaraPT,
                        TurnoPT = x.ProductoTerminado.TurnoPT,
                        OrdenFabricacion = x.ProductoTerminado.OrdenFabricacion,
                        Planta = x.ProductoTerminado.Planta
                    },
                    Tipo = x.Tipo,
                    Comentario = x.Comentario,
                    ComentarioSAP = x.ComentarioSAP

                }).OrderBy(e => e.ProductoTerminado.FechaPesaje).ToList();
                return historialPesadas;
            }
            catch (Exception ex)
            {
                var trace = new StackTrace(ex, true);
                var frame = trace.GetFrames().Last();
                var lineNumber = frame.GetFileLineNumber();
                var fileName = frame.GetFileName();
                List<HistorialPesadas> historialPesadas = new List<HistorialPesadas>();
                return historialPesadas;
            }



        }

        public IList<HistorialPesadas> GetAll(int planta)
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;

            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                List<HistorialPesadas> historialPesadas = _context.HistorialPesadas
                    .Where(x => (x.ProductoTerminado != null && (x.ProductoTerminado.Planta == planta)))
                    .Where(x => (x.ProductoTerminado != null && (x.ProductoTerminado.TurnoPT ?? 0) == 1) || (x.ScrapMolinos != null && x.ScrapMolinos.Turno == 1))
                    .Where(x => (x.ProductoTerminado != null && x.ProductoTerminado.FechaPesaje >= horaInicioTurno1 && x.ProductoTerminado.FechaPesaje <= horaFinTurno1) ||
                          (x.ScrapMolinos != null && x.ScrapMolinos.FechaScrap >= horaInicioTurno1 && x.ScrapMolinos.FechaScrap <= horaFinTurno1) ||
                          (x.ScrapMolinos != null && x.ScrapMolinos.PlantaScrpt >= planta && x.ScrapMolinos.PlantaScrpt <= planta))
                    .Select(x => new HistorialPesadas
                    {
                        Id = x.Id,
                        Id_Scrap = x.Id_Scrap,
                        IdEstatusHP = x.IdEstatusHP,
                        IdEstatusSAP = x.IdEstatusSAP,
                        ComentarioSAP = x.ComentarioSAP,
                        DocNum = x.DocNum,
                        EstadosSAP = new EstadosSAP
                        {
                            Id = x.EstadosSAP.Id,
                            Descripcion = x.EstadosSAP.Descripcion,
                            Estatus = x.EstadosSAP.Estatus
                        },
                        ScrapMolinos = x.ScrapMolinos == null ? null : new ScrapMolinos
                        {
                            Operador = x.ScrapMolinos.Operador,
                            CodigoItem = x.ScrapMolinos.CodigoItem,
                            Peso = float.Parse(x.ScrapMolinos.Peso.ToString()),
                            Familia = x.ScrapMolinos.Familia,
                            Tipo = x.ScrapMolinos.Tipo,
                            SubFamilia = x.ScrapMolinos.SubFamilia,
                            Estado = x.ScrapMolinos.Estado,
                            IdEnvioScrap = x.ScrapMolinos.IdEnvioScrap,
                            EnvioScrap = x.ScrapMolinos.EnvioScrap == null ? null : new EnvioScrap
                            {
                                IdEnvioScrap = x.ScrapMolinos.EnvioScrap.IdEnvioScrap,
                                CodigoItem = x.ScrapMolinos.EnvioScrap.CodigoItem,
                                //Familia = x.ScrapMolinos.EnvioScrap.Familia,
                                Comentarios = x.ScrapMolinos.EnvioScrap.Comentarios,
                                FechaEnvio = x.ScrapMolinos.EnvioScrap.FechaEnvio,
                                Operador = x.ScrapMolinos.EnvioScrap.Operador,
                                Peso = x.ScrapMolinos.EnvioScrap.Peso,
                                Proceso = x.ScrapMolinos.EnvioScrap.Proceso,
                                //SubFamilia = x.ScrapMolinos.EnvioScrap.SubFamilia,
                                //Tipo = x.ScrapMolinos.EnvioScrap.Tipo,
                                Turno = x.ScrapMolinos.EnvioScrap.Turno,
                                Unidad = x.ScrapMolinos.EnvioScrap.Unidad,
                                Linea = x.ScrapMolinos.EnvioScrap.Linea
                            },
                            Turno = x.ScrapMolinos.Turno,
                            Proceso = x.ScrapMolinos.Proceso,
                            Unidad = x.ScrapMolinos.Unidad,
                            FechaScrap = x.ScrapMolinos.FechaScrap,
                            IdScrap = x.ScrapMolinos.IdScrap,
                            AnilloScrpt = float.Parse(x.ScrapMolinos.AnilloScrpt.ToString()),
                            AtadoScrpt = float.Parse(x.ScrapMolinos.AtadoScrpt.ToString()),
                            TaraScrpt = float.Parse(x.ScrapMolinos.TaraScrpt.ToString()),
                            FlejeScrpt = float.Parse(x.ScrapMolinos.FlejeScrpt.ToString()),
                            Cantidad = x.ScrapMolinos.Cantidad
                        },
                        Id_ProductoTerminado = x.Id_ProductoTerminado,
                        ProductoTerminado = x.ProductoTerminado == null ? null : new ProductoTerminado
                        {
                            Id = x.ProductoTerminado.Id,
                            Clasification = x.ProductoTerminado.Clasification,
                            Id_Linea = x.ProductoTerminado.Id_Linea,
                            Lineas = x.ProductoTerminado.Id_Linea == null ? null : new Lineas
                            {
                                Id_linea = x.ProductoTerminado.Lineas.Id_linea,
                                Asignado = x.ProductoTerminado.Lineas.Asignado,
                                Descripcion = x.ProductoTerminado.Lineas.Descripcion,
                                Proceso = x.ProductoTerminado.Lineas.Proceso,
                            },
                            FechaPesaje = x.ProductoTerminado.FechaPesaje,
                            Calidad = x.ProductoTerminado.Calidad,
                            Codigo = x.ProductoTerminado.Codigo,
                            CodigoNombre = x.ProductoTerminado.CodigoNombre,
                            KgxHrbyPt = x.ProductoTerminado.KgxHrbyPt,
                            Proceso = x.ProductoTerminado.Proceso,
                            NumTubos = x.ProductoTerminado.NumTubos,
                            PesoTotal = x.ProductoTerminado.PesoTotal,
                            AnilloPT = x.ProductoTerminado.AnilloPT,
                            AtadoPT = x.ProductoTerminado.AtadoPT,
                            FlejePT = x.ProductoTerminado.FlejePT,
                            FolioPT = x.ProductoTerminado.FolioPT,
                            TaraPT = x.ProductoTerminado.TaraPT,
                            TurnoPT = x.ProductoTerminado.TurnoPT,
                            OrdenFabricacion = x.ProductoTerminado.OrdenFabricacion,
                            Planta = x.ProductoTerminado.Planta
                        },
                        Tipo = x.Tipo,
                        Comentario = x.Comentario
                    }).OrderBy(e => e.ProductoTerminado.FechaPesaje).ToList();

                return historialPesadas;
            }
            else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                List<HistorialPesadas> historialPesadas = _context.HistorialPesadas
                    .Where(x => (x.ProductoTerminado != null && (x.ProductoTerminado.Planta == planta)))
                    .Where(x => (x.ProductoTerminado != null && (x.ProductoTerminado.TurnoPT ?? 0) == 2) || (x.ScrapMolinos != null && x.ScrapMolinos.Turno == 2))
                    .Where(x => (x.ProductoTerminado != null && x.ProductoTerminado.FechaPesaje >= horaInicioTurno2 && x.ProductoTerminado.FechaPesaje <= horaFinTurno2) ||
                          (x.ScrapMolinos != null && x.ScrapMolinos.FechaScrap >= horaInicioTurno2 && x.ScrapMolinos.FechaScrap <= horaFinTurno2) ||
                          (x.ScrapMolinos != null && x.ScrapMolinos.PlantaScrpt >= planta && x.ScrapMolinos.PlantaScrpt <= planta))
                    .Select(x => new HistorialPesadas
                    {
                        Id = x.Id,
                        Id_Scrap = x.Id_Scrap,
                        IdEstatusHP = x.IdEstatusHP,
                        IdEstatusSAP = x.IdEstatusSAP,
                        ComentarioSAP = x.ComentarioSAP,
                        DocNum = x.DocNum,
                        EstadosSAP = new EstadosSAP
                        {
                            Id = x.EstadosSAP.Id,
                            Descripcion = x.EstadosSAP.Descripcion,
                            Estatus = x.EstadosSAP.Estatus
                        },
                        ScrapMolinos = x.ScrapMolinos == null ? null : new ScrapMolinos
                        {
                            Operador = x.ScrapMolinos.Operador,
                            CodigoItem = x.ScrapMolinos.CodigoItem,
                            Peso = float.Parse(x.ScrapMolinos.Peso.ToString()),
                            Familia = x.ScrapMolinos.Familia,
                            Tipo = x.ScrapMolinos.Tipo,
                            SubFamilia = x.ScrapMolinos.SubFamilia,
                            Estado = x.ScrapMolinos.Estado,
                            IdEnvioScrap = x.ScrapMolinos.IdEnvioScrap,
                            EnvioScrap = x.ScrapMolinos.EnvioScrap == null ? null : new EnvioScrap
                            {
                                IdEnvioScrap = x.ScrapMolinos.EnvioScrap.IdEnvioScrap,
                                CodigoItem = x.ScrapMolinos.EnvioScrap.CodigoItem,
                                //Familia = x.ScrapMolinos.EnvioScrap.Familia,
                                Comentarios = x.ScrapMolinos.EnvioScrap.Comentarios,
                                FechaEnvio = x.ScrapMolinos.EnvioScrap.FechaEnvio,
                                Operador = x.ScrapMolinos.EnvioScrap.Operador,
                                Peso = x.ScrapMolinos.EnvioScrap.Peso,
                                Proceso = x.ScrapMolinos.EnvioScrap.Proceso,
                                //SubFamilia = x.ScrapMolinos.EnvioScrap.SubFamilia,
                                //Tipo = x.ScrapMolinos.EnvioScrap.Tipo,
                                Turno = x.ScrapMolinos.EnvioScrap.Turno,
                                Unidad = x.ScrapMolinos.EnvioScrap.Unidad,
                                Linea = x.ScrapMolinos.EnvioScrap.Linea
                            },
                            Turno = x.ScrapMolinos.Turno,
                            Proceso = x.ScrapMolinos.Proceso,
                            Unidad = x.ScrapMolinos.Unidad,
                            FechaScrap = x.ScrapMolinos.FechaScrap,
                            IdScrap = x.ScrapMolinos.IdScrap,
                            AnilloScrpt = float.Parse(x.ScrapMolinos.AnilloScrpt.ToString()),
                            AtadoScrpt = float.Parse(x.ScrapMolinos.AtadoScrpt.ToString()),
                            TaraScrpt = float.Parse(x.ScrapMolinos.TaraScrpt.ToString()),
                            FlejeScrpt = float.Parse(x.ScrapMolinos.FlejeScrpt.ToString()),
                            Cantidad = x.ScrapMolinos.Cantidad
                        },
                        Id_ProductoTerminado = x.Id_ProductoTerminado,
                        ProductoTerminado = x.ProductoTerminado == null ? null : new ProductoTerminado
                        {
                            Id = x.ProductoTerminado.Id,
                            Clasification = x.ProductoTerminado.Clasification,
                            Id_Linea = x.ProductoTerminado.Id_Linea,
                            Lineas = x.ProductoTerminado.Id_Linea == null ? null : new Lineas
                            {
                                Id_linea = x.ProductoTerminado.Lineas.Id_linea,
                                Asignado = x.ProductoTerminado.Lineas.Asignado,
                                Descripcion = x.ProductoTerminado.Lineas.Descripcion,
                                Proceso = x.ProductoTerminado.Lineas.Proceso
                            },
                            FechaPesaje = x.ProductoTerminado.FechaPesaje,
                            Calidad = x.ProductoTerminado.Calidad,
                            Codigo = x.ProductoTerminado.Codigo,
                            CodigoNombre = x.ProductoTerminado.CodigoNombre,
                            KgxHrbyPt = x.ProductoTerminado.KgxHrbyPt,
                            Proceso = x.ProductoTerminado.Proceso,
                            NumTubos = x.ProductoTerminado.NumTubos,
                            PesoTotal = x.ProductoTerminado.PesoTotal,
                            AnilloPT = x.ProductoTerminado.AnilloPT,
                            AtadoPT = x.ProductoTerminado.AtadoPT,
                            FlejePT = x.ProductoTerminado.FlejePT,
                            FolioPT = x.ProductoTerminado.FolioPT,
                            TaraPT = x.ProductoTerminado.TaraPT,
                            TurnoPT = x.ProductoTerminado.TurnoPT,
                            OrdenFabricacion = x.ProductoTerminado.OrdenFabricacion
                        },
                        Tipo = x.Tipo,
                        Comentario = x.Comentario
                    }).OrderBy(e => e.ProductoTerminado.FechaPesaje).ToList();

                return historialPesadas;
            }
        }

        public IList<HistorialPesadas> GetHistoryWeightById(int idHistPesada)
        {
            List<HistorialPesadas> historial = _context.HistorialPesadas
                .Where(x => x.Id == idHistPesada)
                .Select(x => new HistorialPesadas
                {
                    Id = x.Id,
                    Id_Scrap = x.Id_Scrap,
                    IdEstatusHP = x.IdEstatusHP,
                    IdEstatusSAP = x.IdEstatusSAP,
                    ComentarioSAP = x.ComentarioSAP,
                    DocNum = x.DocNum,
                    ScrapMolinos = x.ScrapMolinos == null ? null : new ScrapMolinos
                    {
                        Operador = x.ScrapMolinos.Operador,
                        CodigoItem = x.ScrapMolinos.CodigoItem,
                        Peso = float.Parse(x.ScrapMolinos.Peso.ToString()),
                        Familia = x.ScrapMolinos.Familia,
                        Tipo = x.ScrapMolinos.Tipo,
                        SubFamilia = x.ScrapMolinos.SubFamilia,
                        Estado = x.ScrapMolinos.Estado,
                        IdEnvioScrap = x.ScrapMolinos.IdEnvioScrap,
                        EnvioScrap = x.ScrapMolinos.EnvioScrap == null ? null : new EnvioScrap
                        {
                            IdEnvioScrap = x.ScrapMolinos.EnvioScrap.IdEnvioScrap,
                            CodigoItem = x.ScrapMolinos.EnvioScrap.CodigoItem,
                            //Familia = x.ScrapMolinos.EnvioScrap.Familia,
                            Comentarios = x.ScrapMolinos.EnvioScrap.Comentarios,
                            FechaEnvio = x.ScrapMolinos.EnvioScrap.FechaEnvio,
                            Operador = x.ScrapMolinos.EnvioScrap.Operador,
                            Peso = x.ScrapMolinos.EnvioScrap.Peso,
                            Proceso = x.ScrapMolinos.EnvioScrap.Proceso,
                            //SubFamilia = x.ScrapMolinos.EnvioScrap.SubFamilia,
                            //Tipo = x.ScrapMolinos.EnvioScrap.Tipo,
                            Turno = x.ScrapMolinos.EnvioScrap.Turno,
                            Unidad = x.ScrapMolinos.EnvioScrap.Unidad,
                            Linea = x.ScrapMolinos.EnvioScrap.Linea
                        },
                        Turno = x.ScrapMolinos.Turno,
                        Proceso = x.ScrapMolinos.Proceso,
                        Unidad = x.ScrapMolinos.Unidad,
                        FechaScrap = x.ScrapMolinos.FechaScrap,
                        IdScrap = x.ScrapMolinos.IdScrap,
                        AnilloScrpt = float.Parse(x.ScrapMolinos.AnilloScrpt.ToString()),
                        AtadoScrpt = float.Parse(x.ScrapMolinos.AtadoScrpt.ToString()),
                        TaraScrpt = float.Parse(x.ScrapMolinos.TaraScrpt.ToString()),
                        FlejeScrpt = float.Parse(x.ScrapMolinos.FlejeScrpt.ToString()),
                        Cantidad = x.ScrapMolinos.Cantidad
                    },
                    Id_ProductoTerminado = x.Id_ProductoTerminado,
                    ProductoTerminado = x.ProductoTerminado == null ? null : new ProductoTerminado
                    {
                        Id = x.ProductoTerminado.Id,
                        Clasification = x.ProductoTerminado.Clasification,
                        Id_Linea = x.ProductoTerminado.Id_Linea,
                        Lineas = x.ProductoTerminado.Id_Linea == null ? null : new Lineas
                        {
                            Id_linea = x.ProductoTerminado.Lineas.Id_linea,
                            Asignado = x.ProductoTerminado.Lineas.Asignado,
                            Descripcion = x.ProductoTerminado.Lineas.Descripcion,
                            Proceso = x.ProductoTerminado.Lineas.Proceso
                        },
                        FechaPesaje = x.ProductoTerminado.FechaPesaje,
                        Codigo = x.ProductoTerminado.Codigo,
                        CodigoNombre = x.ProductoTerminado.CodigoNombre,
                        Calidad = x.ProductoTerminado.Calidad,
                        Proceso = x.ProductoTerminado.Proceso,
                        NumTubos = x.ProductoTerminado.NumTubos,
                        PesoTotal = x.ProductoTerminado.PesoTotal,
                        AnilloPT = x.ProductoTerminado.AnilloPT,
                        AtadoPT = x.ProductoTerminado.AtadoPT,
                        FlejePT = x.ProductoTerminado.FlejePT,
                        FolioPT = x.ProductoTerminado.FolioPT,
                        TaraPT = x.ProductoTerminado.TaraPT,
                        TurnoPT = x.ProductoTerminado.TurnoPT,
                        KgxHrbyPt = x.ProductoTerminado.KgxHrbyPt
                    },
                    Tipo = x.Tipo,
                    Comentario = x.Comentario,

                }).ToList();

            return historial;
        }

        public async Task<bool> UpdateSapMessage(string id, string descripcion)
        {
            try
            {
                int identificador = int.Parse(id);
                var sapEnvio = _context.EnvioSAP.Where(x => x.IdentEnvioSAP == identificador).ToList();
                if (sapEnvio.Count > 0)
                {
                    foreach (var item in sapEnvio)
                    {
                        var hPesada = _context.HistorialPesadas.Find(item.IdHistPesada);
                        hPesada.ComentarioSAP = descripcion;
                        hPesada.IdEstatusSAP = 3;

                        _context.Update(hPesada);
                        await _context.SaveChangesAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
