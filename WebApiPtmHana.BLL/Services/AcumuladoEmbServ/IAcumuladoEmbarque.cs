using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.BLL.Services.AcumuladoEmbServ
{
    public interface IAcumuladoEmbarque
    {
        Task<bool> AgregarAcomuladoEmbarque(string proceso, string familia, float acomulado, int planta);
    }
}
