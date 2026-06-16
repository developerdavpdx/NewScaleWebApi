using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.BitacoraHistorialPRepo
{
    public interface IGenericRepositorieHistPesada<TEntityModel> where TEntityModel : class
    {
        IList<TEntityModel> GetLogHistoricalWeightById(int idHPesada);
    }
}
