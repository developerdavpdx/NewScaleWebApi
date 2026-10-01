using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.EnvioSapRepo
{
    public class EnvioSapRepositorie : IGenericRepoEnvioSap<EnvioSAP>
    {
        private readonly ApplicationDbContext _context;
        

        public EnvioSapRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        private static readonly object _idLock = new object();

        public async Task<int> AddToSendToSap(int[] idHistorialPesadas, int preliminar)
        {
            int idFileXML;

            lock (_idLock)
            {
                var lastRecord = _context.EnvioSAP
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();
                idFileXML = lastRecord != null ? lastRecord.IdentEnvioSAP + 1 : 1;
            }

            foreach (var item in idHistorialPesadas)
            {
                EnvioSAP sap = new EnvioSAP()
                {
                    IdentEnvioSAP = idFileXML,
                    IdHistPesada = item,
                    IsPrelim = preliminar
                };
                await _context.AddAsync(sap);
            }

            await _context.SaveChangesAsync();

            return idFileXML;
        }

        public List<EnvioSAP> GetSapListByIdentifier(int identifier)
        {
            List<EnvioSAP> envioSAPs = _context.EnvioSAP
                .Where(x => x.IdentEnvioSAP == identifier)
                .Select(x => new EnvioSAP
                {
                    Id = x.Id,
                    IdentEnvioSAP = x.IdentEnvioSAP,
                    IsPrelim = x.IsPrelim,
                    IdHistPesada = x.IdHistPesada,
                    HistorialPesadas = new HistorialPesadas
                    {
                        Id = x.HistorialPesadas.Id,
                        Id_Scrap = x.HistorialPesadas.Id_Scrap,
                        IdEstatusHP = x.HistorialPesadas.IdEstatusHP,
                        IdEstatusSAP = x.HistorialPesadas.IdEstatusSAP,
                        EstadosSAP = new EstadosSAP
                        {
                            Id = x.HistorialPesadas.EstadosSAP.Id,
                            Descripcion = x.HistorialPesadas.EstadosSAP.Descripcion,
                            Estatus = x.HistorialPesadas.EstadosSAP.Estatus
                        },
                        ScrapMolinos = x.HistorialPesadas.ScrapMolinos == null ? null : new ScrapMolinos
                        {
                            Operador = x.HistorialPesadas.ScrapMolinos.Operador,
                            CodigoItem = x.HistorialPesadas.ScrapMolinos.CodigoItem,
                            Peso = x.HistorialPesadas.ScrapMolinos.Peso,
                            Familia = x.HistorialPesadas.ScrapMolinos.Familia,
                            Tipo = x.HistorialPesadas.ScrapMolinos.Tipo,
                            SubFamilia = x.HistorialPesadas.ScrapMolinos.SubFamilia,
                            Estado = x.HistorialPesadas.ScrapMolinos.Estado,
                            IdEnvioScrap = x.HistorialPesadas.ScrapMolinos.IdEnvioScrap,
                            EnvioScrap = x.HistorialPesadas.ScrapMolinos.EnvioScrap == null ? null : new EnvioScrap
                            {
                                IdEnvioScrap = x.HistorialPesadas.ScrapMolinos.EnvioScrap.IdEnvioScrap,
                                CodigoItem = x.HistorialPesadas.ScrapMolinos.EnvioScrap.CodigoItem,
                                //Familia = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Familia,
                                Comentarios = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Comentarios,
                                FechaEnvio = x.HistorialPesadas.ScrapMolinos.EnvioScrap.FechaEnvio,
                                Operador = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Operador,
                                Peso = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Peso,
                                Proceso = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Proceso,
                                //SubFamilia = x.HistorialPesadas.ScrapMolinos.EnvioScrap.SubFamilia,
                                //Tipo = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Tipo,
                                Turno = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Turno,
                                Unidad = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Unidad,
                                Linea = x.HistorialPesadas.ScrapMolinos.EnvioScrap.Linea
                            },
                            Turno = x.HistorialPesadas.ScrapMolinos.Turno,
                            Proceso = x.HistorialPesadas.ScrapMolinos.Proceso,
                            Unidad = x.HistorialPesadas.ScrapMolinos.Unidad,
                            FechaScrap = x.HistorialPesadas.ScrapMolinos.FechaScrap,
                            IdScrap = x.HistorialPesadas.ScrapMolinos.IdScrap
                        },
                        Id_ProductoTerminado = x.HistorialPesadas.Id_ProductoTerminado,
                        ProductoTerminado = x.HistorialPesadas.ProductoTerminado == null ? null : new ProductoTerminado
                        {
                            Id = x.HistorialPesadas.ProductoTerminado.Id,
                            Clasification = x.HistorialPesadas.ProductoTerminado.Clasification,
                            Id_Linea = x.HistorialPesadas.ProductoTerminado.Id_Linea,
                            Lineas = x.HistorialPesadas.ProductoTerminado.Id_Linea == null ? null : new Lineas
                            {
                                Id_linea = x.HistorialPesadas.ProductoTerminado.Lineas.Id_linea,
                                Asignado = x.HistorialPesadas.ProductoTerminado.Lineas.Asignado,
                                Descripcion = x.HistorialPesadas.ProductoTerminado.Lineas.Descripcion,
                                Proceso = x.HistorialPesadas.ProductoTerminado.Lineas.Proceso
                            },
                            CodigoNombre = x.HistorialPesadas.ProductoTerminado.CodigoNombre,
                            Calidad = x.HistorialPesadas.ProductoTerminado.Calidad,
                            Codigo = x.HistorialPesadas.ProductoTerminado.Codigo,
                            FechaPesaje= x.HistorialPesadas.ProductoTerminado.FechaPesaje,
                            KgxHrbyPt = x.HistorialPesadas.ProductoTerminado.KgxHrbyPt,
                            Proceso = x.HistorialPesadas.ProductoTerminado.Proceso,
                            NumTubos = x.HistorialPesadas.ProductoTerminado.NumTubos,
                            PesoTotal = x.HistorialPesadas.ProductoTerminado.PesoTotal,
                            AnilloPT = x.HistorialPesadas.ProductoTerminado.AnilloPT,
                            AtadoPT = x.HistorialPesadas.ProductoTerminado.AtadoPT,
                            FlejePT = x.HistorialPesadas.ProductoTerminado.FlejePT,
                            FolioPT = x.HistorialPesadas.ProductoTerminado.FolioPT,
                            TaraPT = x.HistorialPesadas.ProductoTerminado.TaraPT,
                            TurnoPT = x.HistorialPesadas.ProductoTerminado.TurnoPT,
                            OrdenFabricacion = x.HistorialPesadas.ProductoTerminado.OrdenFabricacion
                        },
                        Tipo = x.HistorialPesadas.Tipo,
                        Comentario = x.HistorialPesadas.Comentario
                    }
                }).ToList();

            return envioSAPs;
        }
    }
}
