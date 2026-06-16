using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.EnvioSapRepo
{
    public interface IGenericRepoEnvioSap<TEntityModel> where TEntityModel : class
    {
        Task<int> AddToSendToSap(int[] idHistorialPesadas, int preliminar);
        List<TEntityModel> GetSapListByIdentifier(int identifier);
    }
}
