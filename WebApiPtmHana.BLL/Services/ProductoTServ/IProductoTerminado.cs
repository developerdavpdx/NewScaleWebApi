using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.ProductoTServ
{
    public interface IProductoTerminado
    {
        IList<ProductoTerminado> GetAllPtIny();
        IList<ProductoTerminado> GetAllPtPvc();
        IList<ProductoTerminado> GetPtPvcByLinea(int numLinea);
        IList<ProductoTerminado> GetPtInyByLinea(int numLinea);
        IList<ProductoTerminado> FilterFinishedProduct(int? turno, string? codigo, string? fechaInicio, string? fechaFin, string proceso);
    }
}
