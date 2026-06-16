using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.AcumuladoEmbRepo
{
    public interface IGenericRepoAcumuladoEmb<TEntityModel> where TEntityModel : class
    {
        Task<bool> AgregarAcomuladoEmbarque(string proceso, string familia, float acomulado, int planta);
    }
}
