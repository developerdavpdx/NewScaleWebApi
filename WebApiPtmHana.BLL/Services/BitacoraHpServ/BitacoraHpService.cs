using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.BitacoraHistorialPRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.BitacoraHpServ
{
    public class BitacoraHpService : IBitacoraHPesadas
    {
        private readonly IGenericRepositorieHistPesada<BitHistorialPesadas> _repositorie;
        public BitacoraHpService(IGenericRepositorieHistPesada<BitHistorialPesadas> repositorie)
        {
            _repositorie = repositorie;
        }

        public IList<BitHistorialPesadas> GetHistoricalProducts(int idHPesadas)
        {
            return _repositorie.GetLogHistoricalWeightById(idHPesadas);
        }
    }
}
