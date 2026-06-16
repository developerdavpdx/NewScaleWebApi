using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.ProductoTRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.ProductoTServ
{
    public class ProductoTService : IProductoTerminado
    {
        private readonly IGenericRepositorieProductoT<ProductoTerminado> _repositorie;
        public ProductoTService(IGenericRepositorieProductoT<ProductoTerminado> repositorie) 
        {
            _repositorie = repositorie;
        }

        public IList<ProductoTerminado> FilterFinishedProduct(int? turno, string? codigo, string? fechaInicio, string? fechaFin, string proceso)
        {
            return _repositorie.FilterFinishedProduct(turno, codigo, fechaInicio, fechaFin, proceso);
        }

        public IList<ProductoTerminado> GetAllPtIny()
        {
            return _repositorie.GetAllPtIny();
        }

        public IList<ProductoTerminado> GetAllPtPvc()
        {
            return _repositorie.GetAllPtPvc();
        }

        public IList<ProductoTerminado> GetPtInyByLinea(int numLinea)
        {
            return _repositorie.GetPtInyByLinea(numLinea);
        }

        public IList<ProductoTerminado> GetPtPvcByLinea(int numLinea)
        {
            return _repositorie.GetPtPvcByLinea(numLinea);
        }
    }
}
