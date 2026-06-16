using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.BitacoraLiniaRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.BitacoraLServ
{
    public class BitacoraLService : IBitacoraLinia
    {
        private readonly IGenericRepoBitLinia<BitacoraAsignacionLinia> _repository;
        public BitacoraLService(IGenericRepoBitLinia<BitacoraAsignacionLinia> repositories)
        {
            _repository = repositories;
        }

        public IList<BitacoraAsignacionLinia> GetBitAsignByLinie(int linie, string planta)
        {
            return _repository.GetBitAsignByLinie(linie, planta);
        }
    }
}
