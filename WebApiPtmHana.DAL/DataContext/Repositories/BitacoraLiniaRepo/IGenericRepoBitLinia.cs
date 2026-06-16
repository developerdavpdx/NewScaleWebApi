using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.BitacoraLiniaRepo
{
    public interface IGenericRepoBitLinia<TEntityModel> where TEntityModel : class
    {
        IList<TEntityModel> GetBitAsignByLinie(int linea, string planta);
    }
}
