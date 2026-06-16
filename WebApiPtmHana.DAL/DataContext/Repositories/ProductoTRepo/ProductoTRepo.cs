using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.ProductoTRepo
{
    public class ProductoTRepo : IGenericRepositorieProductoT<ProductoTerminado>
    {
        private readonly ApplicationDbContext _context;
        private readonly string connectionStringSQL;
        public ProductoTRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ProductoTerminado> FilterFinishedProduct(int? turno, string? codigo, string? fechaInicio, string? fechaFin, string proceso)
        {
            var productoTerminado = _context.ProductoTerminados
                .Where(x => x.Proceso == proceso)
                .Where(x => turno == null || x.TurnoPT == turno)
                .Where(x => codigo ==null || x.Codigo == codigo)
                .Where(x => (fechaInicio == null && fechaFin == null) ||
                    (x.FechaPesaje >= DateTime.Parse(fechaInicio) && x.FechaPesaje <= DateTime.Parse(fechaFin)))
                    .Select(x => new ProductoTerminado
                    {
                        Codigo = x.Codigo,
                        Calidad = x.Calidad,
                        CodigoNombre = x.CodigoNombre,
                        Proceso = x.Proceso,
                        Id = x.Id,
                        Id_Linea = x.Id_Linea,
                        Lineas = x.Lineas == null ? null : new Lineas
                        {
                            Id_linea = x.Lineas.Id_linea,
                            Asignado = x.Lineas.Asignado,
                            Descripcion = x.Lineas.Descripcion,
                            Proceso = x.Lineas.Proceso
                        },
                        Clasification = x.Clasification,
                        FechaPesaje = x.FechaPesaje,
                        NumTubos = x.NumTubos,
                        PesoTotal = x.PesoTotal,
                        KgxHrbyPt = x.KgxHrbyPt,
                        AnilloPT = x.AnilloPT,
                        AtadoPT = x.AtadoPT,
                        FlejePT = x.FlejePT,
                        FolioPT = x.FolioPT,
                        TaraPT = x.TaraPT,
                        TurnoPT = x.TurnoPT
                    }).ToList();

            return productoTerminado;
        }

        public IList<ProductoTerminado> GetAllPtIny()
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;
            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                List<ProductoTerminado> productosIny = _context.ProductoTerminados
                    .Where(x => x.Proceso == "INY")
                    .Where(x => (x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1))
                    .ToList();

                return productosIny;
            }
            else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                List<ProductoTerminado> productosIny = _context.ProductoTerminados
                    .Where(x => x.Proceso == "INY")
                    .Where(x => (x.FechaPesaje >= horaInicioTurno2 && x.FechaPesaje <= horaFinTurno2))
                    .ToList();

                return productosIny;
            }
        }

        public IList<ProductoTerminado> GetAllPtPvc()
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;
            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                List<ProductoTerminado> productosIny = _context.ProductoTerminados
                    .Where(x => x.Proceso == "PPVC")
                    .Where(x => (x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1))
                    .ToList();

                return productosIny;
            }
            else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                List<ProductoTerminado> productosIny = _context.ProductoTerminados
                    .Where(x => x.Proceso == "PPVC")
                    .Where(x => (x.FechaPesaje >= horaInicioTurno2 && x.FechaPesaje <= horaFinTurno2))
                    .ToList();

                return productosIny;
            }
        }

        public IList<ProductoTerminado> GetPtInyByLinea(int numLinea)
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;

            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                List<ProductoTerminado> productosIny = _context.ProductoTerminados
                .Where(x => x.Proceso == "INY" && x.Id_Linea == numLinea)
                .Where(x => (x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1))
                .ToList();

                return productosIny;
            } else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                List<ProductoTerminado> productosIny = _context.ProductoTerminados
                .Where(x => x.Proceso == "INY" && x.Id_Linea == numLinea)
                .Where(x => (x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1))
                .ToList();

                return productosIny;
            }
        }

        public IList<ProductoTerminado> GetPtPvcByLinea(int numLinea)
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;

            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                List<ProductoTerminado> productosPvc = _context.ProductoTerminados
                .Where(x => x.Proceso == "PPVC" && x.Id_Linea == numLinea)
                .Where(x => (x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1))
                .ToList();

                return productosPvc;
            } else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                List<ProductoTerminado> productosPvc = _context.ProductoTerminados
                .Where(x => x.Proceso == "PPVC" && x.Id_Linea == numLinea)
                .Where(x => (x.FechaPesaje >= horaInicioTurno1 && x.FechaPesaje <= horaFinTurno1))
                .ToList();

                return productosPvc;
            }
        }
    }
}
