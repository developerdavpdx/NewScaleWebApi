using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.EnvioSapServ
{
    public interface ISapEnvio
    {
        Task<int> AddRecordToSendSAP(int[] idHistorialPesadas, int preliminar);
        List<EnvioSAP> GetListToSend(int preliminar);
        
    }
}
