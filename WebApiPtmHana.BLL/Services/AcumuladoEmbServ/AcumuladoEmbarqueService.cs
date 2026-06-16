using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.AcumuladoEmbRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.AcumuladoEmbServ
{
    public class AcumuladoEmbarqueService : IAcumuladoEmbarque
    {
        IGenericRepoAcumuladoEmb<AcumuladoEmbarque> _repositorie;
        public AcumuladoEmbarqueService(IGenericRepoAcumuladoEmb<AcumuladoEmbarque> repositorie)
        {
            _repositorie = repositorie;
        }
        public async Task<bool> AgregarAcomuladoEmbarque(string proceso, string familia, float acomulado, int planta)
        {
            return await _repositorie.AgregarAcomuladoEmbarque(proceso, familia, acomulado, planta);
        }
    }
}
