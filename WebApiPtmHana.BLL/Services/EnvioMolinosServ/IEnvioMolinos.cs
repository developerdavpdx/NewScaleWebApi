using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.EnvioMolinosServ
{
    public interface IEnvioMolinos
    {
        IList<EnvioScrap> GetEnvioScrapById(int id);
        Task<string> SendToMills(EnvioScrap entityModel);
        int GetNextIdEnvioMolinos();
        IList<EnvioScrap> GetEnvioScrap(string planta);
        IList<EnvioScrap> FilterReciboScrap(string? fechaInicio, string? fechaFin, int? turno, string planta);
    }
}
