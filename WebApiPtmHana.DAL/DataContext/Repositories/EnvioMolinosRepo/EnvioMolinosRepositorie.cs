using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.EnvioMolinosRepo
{
    public class EnvioMolinosRepositorie : IGenericRepoEnvioMolinos<EnvioScrap>
    {
        private readonly ApplicationDbContext _context;
        public EnvioMolinosRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<EnvioScrap> FilterReciboScrap(string? fechaInicio, string? fechaFin, int? turno, string planta)
        {
            
            IList<EnvioScrap> scrapMolinos = _context.EnvioScraps
                   .Where(x => (turno == null || x.Turno == turno))
                   .Where(x => x.FechaEnvio >= DateTime.Parse(fechaInicio) && x.FechaEnvio <= DateTime.Parse(fechaFin))
                   .Where(x => x.ScrapPlanta == int.Parse(planta))
                   .ToList();

            return scrapMolinos;
        }

        public IList<EnvioScrap> GetEnvioScrapById(int id)
        {
            IList<EnvioScrap> envioScrap = _context.EnvioScraps.Where(x => x.IdEnvioScrap == id).ToList();
            return envioScrap;
        }

        public IList<EnvioScrap> GetEnvioScrapByTurno(string planta)
        {
            DateTime horaInicioTurno1 = DateTime.Today.AddHours(4.5); // Fecha y hora de inicio del turno 1 (hoy a las 4:30 am)
            DateTime horaFinTurno1 = DateTime.Today.AddHours(16.5); // Fecha y hora de fin del turno 1 (hoy a las 4:30 pm)
            DateTime horaInicioTurno2 = DateTime.Today.AddHours(16.5); // Fecha y hora de inicio del turno 2 (hoy a las 4:30 pm)
            DateTime horaFinTurno2 = DateTime.Today.AddDays(1).AddHours(4.5); // Fecha y hora de fin del turno 2 (mañana a las 4:30 am)
            DateTime horaActual = DateTime.Now;

            if (horaActual >= horaInicioTurno1 && horaActual <= horaFinTurno1)
            {
                IList<EnvioScrap> scrapMolinos = _context.EnvioScraps
                    .Where(x => x.FechaEnvio >= horaInicioTurno1 && x.FechaEnvio <= horaFinTurno1 && x.ScrapPlanta == int.Parse(planta)).ToList();

                return scrapMolinos;
            }
            else
            {
                if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                {
                    horaInicioTurno2 = horaInicioTurno2.AddDays(-1);
                    horaFinTurno2 = horaFinTurno2.AddDays(-1);
                }

                IList<EnvioScrap> scrapMolinos = _context.EnvioScraps
                    .Where(x => x.FechaEnvio >= horaInicioTurno1 && x.FechaEnvio <= horaFinTurno1 && x.ScrapPlanta == int.Parse(planta)).ToList();

                return scrapMolinos;
            }
        }

        public int GetNextIdEnvioScrap()
        {
            EnvioScrap registro = _context.EnvioScraps.OrderByDescending(x => x.IdEnvioScrap).FirstOrDefault();
            if (registro == null)
                return 0;

            var id = registro.IdEnvioScrap;
            return id;
        }

        public async Task<string> SendToMills(EnvioScrap entityModel)
        {
            try
            {
                EnvioScrap envioScrap = new EnvioScrap
                {
                    //Tipo = entityModel.Tipo,
                    //SubFamilia = entityModel.SubFamilia,
                    CodigoItem = entityModel.CodigoItem,
                    Familia = entityModel.Familia,
                    Peso = entityModel.Peso,
                    Operador = entityModel.Operador,
                    Comentarios = entityModel.Comentarios,
                    Turno = entityModel.Turno,
                    Proceso = entityModel.Proceso,
                    Unidad = entityModel.Unidad,
                    FechaEnvio = entityModel.FechaEnvio,
                    Linea = entityModel.Linea,
                    ScrapPlanta = entityModel.ScrapPlanta,
                    CodigoLinea = entityModel.CodigoLinea
                };
                
                // Agrega el objeto al contexto
                _context.EnvioScraps.AddAsync(envioScrap);
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return "true";
            }
            catch (Exception ex)
            {
                //armado de la excepcion
                StringBuilder error = new StringBuilder();
                error.Append(ex.InnerException != null ? ex.InnerException.ToString() : string.Empty);
                error.Append(ex.Message != null ? ex.Message.ToString() : string.Empty);
                return error.ToString();
            }
        }
    }
}
