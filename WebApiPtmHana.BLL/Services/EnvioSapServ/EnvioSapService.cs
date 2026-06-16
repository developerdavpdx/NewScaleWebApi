using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.EnvioSapRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.EnvioSapServ
{
    public class EnvioSapService : ISapEnvio
    {
        private readonly IGenericRepoEnvioSap<EnvioSAP> _repositorie;
        public EnvioSapService(IGenericRepoEnvioSap<EnvioSAP> repositorie)
        {
            _repositorie = repositorie;
        }

        public async Task<int> AddRecordToSendSAP(int[] idHistorialPesadas, int preliminar)
        {
            return await _repositorie.AddToSendToSap(idHistorialPesadas, preliminar);
        }

        public List<EnvioSAP> GetListToSend(int preliminar)
        {
            return _repositorie.GetSapListByIdentifier(preliminar);
        }

    }
}
