using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.BLL.Services.TarasServ
{
    public interface ITaras
    {
        Task<bool> UpdateTaras(string jsonTaras);
    }
}
