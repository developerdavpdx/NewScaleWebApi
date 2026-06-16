using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.EnvioMolinosRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.EnvioMolinosServ
{
    public class EnvioMolinosService : IEnvioMolinos
    {
        private readonly IGenericRepoEnvioMolinos<EnvioScrap> _repositorie;

        public EnvioMolinosService(IGenericRepoEnvioMolinos<EnvioScrap> repositorie)
        {
            _repositorie = repositorie;
        }

        public IList<EnvioScrap> GetEnvioScrap(string planta)
        {
            return _repositorie.GetEnvioScrapByTurno(planta);
        }

        public IList<EnvioScrap> GetEnvioScrapById(int id)
        {
            return _repositorie.GetEnvioScrapById(id);
        }

        public int GetNextIdEnvioMolinos()
        {
            return _repositorie.GetNextIdEnvioScrap();
        }

        public async Task<string> SendToMills(EnvioScrap entityModel)
        {
            return await _repositorie.SendToMills(entityModel);
        }

        public IList<EnvioScrap> FilterReciboScrap(string? fechaInicio, string? fechaFin, int? turno, string planta)
        {
            return _repositorie.FilterReciboScrap(fechaInicio, fechaFin, turno, planta);
        }
    }
}
