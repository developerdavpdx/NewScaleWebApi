using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.ScrapServ
{
    public interface IScrap
    {
        IList<ScrapMolinos> GetAllScrapMolinos(string planta);
        Task<bool> AddNewScrap(ScrapMolinos scrap);
        Task<bool> AddRemolidoScrap(ScrapMolinos scrap);
        int GetNextIdScrap();
        IList<ScrapMolinos> FilterScrap(string? fechaInicio, string? fechaFin, int? turno, string? estado, string planta);
        IEnumerable<object> GetSumLineas(int turno, DateTime horaTurnoStart, DateTime horaTurnoEnd);
    }
}
