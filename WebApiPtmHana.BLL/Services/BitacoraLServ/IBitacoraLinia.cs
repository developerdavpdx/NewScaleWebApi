using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.BitacoraLServ
{
    public interface IBitacoraLinia
    {
        IList<BitacoraAsignacionLinia> GetBitAsignByLinie(int linie, string planta);
    }
}
