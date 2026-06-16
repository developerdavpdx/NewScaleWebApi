using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.ScarpRepo
{
    public interface IGenericRepositorieScrap<EntityModel> where EntityModel : class
    {
        IList<EntityModel> GetAllScrapMolinos(string planta);
        Task<bool> AddNewScrap(EntityModel entity);
        Task<bool> AddRemolido(EntityModel entity);
        int GetNextIdScrap();
        IList<EntityModel> FilterScrap(string? fechaInicio, string? fechaFin, int? turno, string? estado, string planta);
        IEnumerable<object> GetSumLineas(int turno, DateTime horaTurnoStart, DateTime horaTurnoEnd);
    }
}
