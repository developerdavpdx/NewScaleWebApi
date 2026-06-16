using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.TicketsRepo
{
    public class TicketsRepositorie : IGenericRepoTickets<TicketsInfo>
    {
        private readonly ApplicationDbContext _context;
        public TicketsRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<TicketsInfo> FilterTickets(string? folio, int? turno, string? fechaInicio, string? fechaFin, int planta)
        {
            List<TicketsInfo> ticketsInfos = _context.TicketsInfos
                .Where(x => turno == null || x.Turno == turno)
                .Where(x => x.PlantaTicket == planta)
                .Where(x => folio == null || x.Folio == folio)
                .Where(x => (fechaInicio == null && fechaFin == null) || 
                (x.FechaPesaje >= DateTime.Parse(fechaInicio) && x.FechaPesaje <= DateTime.Parse(fechaFin)))
                .Select(x => new TicketsInfo
                    {
                        IdTicket = x.IdTicket,
                        Categoria = x.Categoria,
                        Folio = x.Folio,
                        Id_Linea = x.Id_Linea,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        Medida = x.Medida,
                        NumTubos = x.NumTubos,
                        Operacion = x.Operacion,
                        OrdenFabricacion = x.OrdenFabricacion,
                        PesoNeto = x.PesoNeto,
                        Porcentaje = x.Porcentaje,
                        Turno = x.Turno,
                        Linea = new Lineas
                        {
                            Id_linea = x.Linea.Id_linea,
                            Asignado = x.Linea.Asignado,
                            Descripcion = x.Linea.Descripcion
                        },
                        AsignadoTicket = x.AsignadoTicket,
                        FechaPesaje = x.FechaPesaje,
                        PlantaTicket = x.PlantaTicket
                    }).ToList();

            return ticketsInfos;
        }

        public IList<TicketsInfo> GetAll(int planta)
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5);
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5);
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5);
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5);
            DateTime horaActual = DateTime.Now;
            if(horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {

                IList<TicketsInfo> TicketsInfos = _context.TicketsInfos
                    .Where(x => x.PlantaTicket == planta)
                    .Where(x => x.Linea != null)
                    .Where(x => x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1)
                    .Select(x => new TicketsInfo
                    {
                        IdTicket = x.IdTicket,
                        Categoria = x.Categoria,
                        Folio = x.Folio,
                        Id_Linea = x.Id_Linea,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        Medida = x.Medida,
                        NumTubos = x.NumTubos,
                        Operacion = x.Operacion,
                        OrdenFabricacion = x.OrdenFabricacion,
                        PesoNeto = x.PesoNeto,
                        Porcentaje = x.Porcentaje,
                        Turno = x.Turno,
                        Linea = new Lineas
                        {
                            Id_linea = x.Linea.Id_linea,
                            Asignado = x.Linea.Asignado,
                            Descripcion = x.Linea.Descripcion
                        },
                        AsignadoTicket = x.AsignadoTicket,
                        FechaPesaje = x.FechaPesaje,
                        PlantaTicket = x.PlantaTicket
                    }).ToList();

                return TicketsInfos;
            } else
            {
                if(horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                IList<TicketsInfo> TicketsInfos = _context.TicketsInfos
                    .Where(x => x.PlantaTicket == planta)
                    .Where(x => x.Linea != null)
                    .Where(x => x.FechaPesaje >= horaInicioTurno2 && x.FechaPesaje <= horaFinTurno2)
                    .Select(x => new TicketsInfo
                    {
                        IdTicket = x.IdTicket,
                        Categoria = x.Categoria,
                        Folio = x.Folio,
                        Id_Linea = x.Id_Linea,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        Medida = x.Medida,
                        NumTubos = x.NumTubos,
                        Operacion = x.Operacion,
                        OrdenFabricacion = x.OrdenFabricacion,
                        PesoNeto = x.PesoNeto,
                        Porcentaje = x.Porcentaje,
                        Turno = x.Turno,
                        Linea = new Lineas
                        {
                            Id_linea = x.Linea.Id_linea,
                            Asignado = x.Linea.Asignado,
                            Descripcion = x.Linea.Descripcion
                        },
                        AsignadoTicket = x.AsignadoTicket,
                        FechaPesaje = x.FechaPesaje,
                        PlantaTicket = x.PlantaTicket
                    }).ToList();

                return TicketsInfos;
            }

        }

        public IList<TicketsInfo> GetPrintTickeyById(int id)
        {
            try
            {
                IList<TicketsInfo> ticket = _context.TicketsInfos
                .Where(x => x.IdTicket == id)
                .Select(x => new TicketsInfo
                {
                    IdTicket = x.IdTicket,
                    Categoria = x.Categoria,
                    Folio = x.Folio,
                    Id_Linea = x.Id_Linea,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    Medida = x.Medida,
                    NumTubos = x.NumTubos,
                    Operacion = x.Operacion,
                    OrdenFabricacion = x.OrdenFabricacion,
                    PesoNeto = x.PesoNeto,
                    Porcentaje = x.Porcentaje,
                    Turno = x.Turno,
                    Linea = new Lineas
                    {
                        Id_linea = x.Linea.Id_linea,
                        Asignado = x.Linea.Asignado,
                        Descripcion = x.Linea.Descripcion
                    },
                    FechaPesaje = x.FechaPesaje,
                    AsignadoTicket = x.AsignadoTicket
                }).ToList();

                return ticket;
            }
            catch (Exception ex)
            {
                IList<TicketsInfo> TicketsInfos = new List<TicketsInfo>();
                return TicketsInfos;
            }
        }

        public IList<TicketsInfo> GetTicketByLinie(int linie)
        {
            try
            {
                IList<TicketsInfo> TicketsInfos = _context.TicketsInfos
                    .Where(x => x.Id_Linea == linie && x.Linea != null && x.AsignadoTicket == true)
                    .Select(x => new TicketsInfo
                    {
                        IdTicket = x.IdTicket,
                        Categoria = x.Categoria,
                        Folio = x.Folio,
                        Id_Linea = x.Id_Linea,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        Medida = x.Medida,
                        NumTubos = x.NumTubos,
                        Operacion = x.Operacion,
                        OrdenFabricacion = x.OrdenFabricacion,
                        PesoNeto = x.PesoNeto,
                        Porcentaje = x.Porcentaje,
                        Turno = x.Turno,
                        Linea = new Lineas
                        {
                            Id_linea = x.Linea.Id_linea,
                            Asignado = x.Linea.Asignado,
                            Descripcion = x.Linea.Descripcion
                        },
                        AsignadoTicket = x.AsignadoTicket,
                        FechaPesaje = x.FechaPesaje
                    }).ToList();

                return TicketsInfos;
            }
            catch (Exception ex)
            {
                IList<TicketsInfo> TicketsInfos = new List<TicketsInfo>();
                return TicketsInfos;
            }
        }

        public IList<TicketsInfo> GetTickeyById(int id)
        {
            IList<TicketsInfo> ticket = _context.TicketsInfos
                .Where(x => x.IdTicket == id)
                .Select(x => new TicketsInfo
                {
                    IdTicket = x.IdTicket,
                    Categoria = x.Categoria,
                    Folio = x.Folio,
                    Id_Linea = x.Id_Linea,
                    ItemCode = x.ItemCode,
                    ItemName = x.ItemName,
                    Medida = x.Medida,
                    NumTubos = x.NumTubos,
                    Operacion = x.Operacion,
                    OrdenFabricacion = x.OrdenFabricacion,
                    PesoNeto = x.PesoNeto,
                    Porcentaje = x.Porcentaje,
                    Turno = x.Turno,
                    Linea = new Lineas
                    {
                        Id_linea = x.Linea.Id_linea,
                        Asignado = x.Linea.Asignado,
                        Descripcion = x.Linea.Descripcion
                    },
                    FechaPesaje = x.FechaPesaje,
                    AsignadoTicket = x.AsignadoTicket,
                    PesoTubo = x.PesoTubo
                }).ToList();

            return ticket;
        }

        public Task<bool> InsertTickey(TicketsInfo entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTicket(int idTicket, float peso,float pesoBascula, float porcentaje, bool calidad, decimal pesotubos, string tubos, int planta)
        {
            try
            {
                var x = _context.TicketsInfos.Where(x => x.IdTicket == idTicket).ToList();
                TicketsInfo ticketsInfo = _context.TicketsInfos.Find(idTicket);
                var lineaInfo = _context.Lineas.Where(x => x.NumLinea == ticketsInfo!.Id_Linea && x.Planta == planta).ToList();
                List<SkuInfo> skuInfos = _context.SkuInfos.Where(x => x.ItemCode == ticketsInfo.ItemCode && x.Planta == planta).ToList();

                if (ticketsInfo == null)
                    return false;

                List<SkuInfo> productos = _context.SkuInfos.Where(x => x.ItemCode == ticketsInfo.ItemCode).ToList();

                if (productos.Count > 0)
                {
                    if(tubos == "" || tubos == null)
                    {
                        ticketsInfo.NumTubos = int.Parse(skuInfos[0].MultiploCant.ToString());
                    } else
                    {
                        ticketsInfo.NumTubos = int.Parse(tubos);
                    }
                    //EL PESO DE LA BASCULA SE AGREGO EN TICKETS INFO
                    ticketsInfo.PesoNeto = peso;
                    ticketsInfo.PesoBascula = pesoBascula;
                    ticketsInfo.Medida = "KG";
                    ticketsInfo.FechaPesaje = DateTime.Now;
                    ticketsInfo.Porcentaje = porcentaje;
                    ticketsInfo.PesoTubo = pesotubos;
                    ticketsInfo.PlantaTicket = planta;

                    _context.Update(ticketsInfo);
                    await _context.SaveChangesAsync();


                    int NumTubosFinales = 0;

                    if (tubos == "" || tubos == null)
                    {
                        NumTubosFinales = int.Parse(skuInfos[0].MultiploCant.ToString());
                    }
                    else
                    {
                        NumTubosFinales = int.Parse(tubos);
                    }

                    ProductoTerminado productoT = new ProductoTerminado()
                    {
                        Codigo = skuInfos[0].ItemCode,
                        CodigoNombre = skuInfos[0].ItemName,
                        Clasification = "Producto Terminado",
                        FechaPesaje = DateTime.Now,
                        Id_Linea = ticketsInfo.Id_Linea,
                        NumTubos = NumTubosFinales,
                        TurnoPT = ticketsInfo.Turno,
                        PesoTotal = peso,
                        TaraPT = skuInfos[0].Variable,
                        AtadoPT = skuInfos[0].MultiploCategoria == "Atado" ? skuInfos[0].MultiploCant : 0,
                        FolioPT = Convert.ToInt32(lineaInfo[0].FolioSAP),
                        FlejePT = skuInfos[0].FijoCategoria == "Fleje" ? skuInfos[0].Variable : 0,
                        AnilloPT = skuInfos[0].FijoCategoria == "Fleje" ? skuInfos[0].Variable : 0,
                        KgxHrbyPt = lineaInfo[0].KgXHr,
                        Calidad = calidad,
                        Proceso = lineaInfo[0].Proceso,
                        OrdenFabricacion = ticketsInfo.OrdenFabricacion,
                        Planta = planta
                    };

                    await _context.ProductoTerminados.AddAsync(productoT);
                    await _context.SaveChangesAsync();

                    HistorialPesadas historialPesadas = new HistorialPesadas()
                    {
                        Id_ProductoTerminado = productoT.Id,
                        Tipo = "PT",
                        IdEstatusHP = 1,
                        IdEstatusSAP = 2
                    };

                    await _context.AddAsync(historialPesadas);

                    await _context.SaveChangesAsync();
                    return true;
                } else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
