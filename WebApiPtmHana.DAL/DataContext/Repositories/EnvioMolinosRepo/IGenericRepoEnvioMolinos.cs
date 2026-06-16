using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.EnvioMolinosRepo
{
    public interface IGenericRepoEnvioMolinos<TEntityModel> where TEntityModel : class
    {
        IList<TEntityModel> GetEnvioScrapById(int id);
        Task <string> SendToMills(TEntityModel entityModel);
        int GetNextIdEnvioScrap();
        IList<TEntityModel> GetEnvioScrapByTurno(string planta);
        IList<TEntityModel> FilterReciboScrap(string? fechaInicio, string? fechaFin, int? turno, string planta);
    }
}
