using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.ProductoTRepo
{
    public interface IGenericRepositorieProductoT<EntityModel> where EntityModel : class
    {
        IList<EntityModel> GetAllPtIny();
        IList<EntityModel> GetAllPtPvc();
        IList<EntityModel> GetPtPvcByLinea(int numLinea);
        IList<EntityModel> GetPtInyByLinea(int numLinea);
        IList<EntityModel> FilterFinishedProduct(int? turno, string? codigo, string? fechaInicio, string? fechaFin, string proceso);
    }
}
