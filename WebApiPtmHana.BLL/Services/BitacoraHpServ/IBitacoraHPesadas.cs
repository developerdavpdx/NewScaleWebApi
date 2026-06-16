using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.BitacoraHpServ
{
    public interface IBitacoraHPesadas
    {
        IList<BitHistorialPesadas> GetHistoricalProducts(int idHPesadas);
    }
}
