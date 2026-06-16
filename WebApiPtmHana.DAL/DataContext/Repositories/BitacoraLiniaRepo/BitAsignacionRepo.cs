using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.BitacoraLiniaRepo
{
    public class BitAsignacionRepo : IGenericRepoBitLinia<BitacoraAsignacionLinia>
    {
        private readonly ApplicationDbContext _appContext;
        public BitAsignacionRepo(ApplicationDbContext appContext)
        {
            _appContext = appContext;
        }
        public IList<BitacoraAsignacionLinia> GetBitAsignByLinie(int linea, string planta)
        {
            //El historial abarcara de hoy, contando 30 dias atras
            DateTime FechaInicio = DateTime.Today.AddDays(-30);
            DateTime FechaActual = DateTime.Now;

            List<BitacoraAsignacionLinia> bitacora = _appContext.BitacoraAsignacionLinias
            .Where(x => x.Linea == linea)
            .Where(x => x.Planta == planta)
            .Where(x => x.FechaAsignacion >= FechaInicio && x.FechaAsignacion <= FechaActual)
            .OrderByDescending(x => x.FechaAsignacion) // Ordenar por FechaAsignacion en orden descendente
            .ToList();

            return bitacora;

            //-->> Codigo deprecado el 19/07/2024, si no se vuelve a usar, se puede borrar

            //DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            //DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            //DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            //DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            //DateTime horaActual = DateTime.Now;

            ////turno 1
            //if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            //{
            //    List<BitacoraAsignacionLinia> bitacora = _appContext.BitacoraAsignacionLinias
            //    .Where(x=> x.Linea == linea)
            //    .Where(x => x.Planta == planta)
            //    .Where(x => x.FechaAsignacion >= horaInicioTurno1 && x.FechaAsignacion <= horaFinTurno1)
            //    .ToList();

            //    return bitacora;
            //}
            ////turno 2
            //else
            //{
            //    /*si la hora actual pertenece al turno 2, pero ya es la segunda mitad del turno 2 y es 'el dia siguiente',
            //    reasignamos los valores de la horaFinTurno2 y horaInicioTurno2 para qe sigan coincidiendo*/
            //    if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
            //    {
            //        horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
            //        horaFinTurno2 = horaFinTurno2.AddDays(-1);
            //    }

            //    List<BitacoraAsignacionLinia> bitacora = _appContext.BitacoraAsignacionLinias
            //   .Where(x => x.Linea == linea)
            //   .Where(x => x.Planta == planta)
            //   .Where(x => x.FechaAsignacion >= horaInicioTurno2 && x.FechaAsignacion <= horaFinTurno2)
            //   .ToList();

            //    return bitacora;
            //}
        }
    }
}
