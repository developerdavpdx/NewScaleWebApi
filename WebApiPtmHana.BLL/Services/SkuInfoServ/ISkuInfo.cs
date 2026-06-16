using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.SkuInfoServ
{
    public interface ISkuInfo
    {
        Task<IList<SkuInfo>> GetAllSkuInfos(int planta);
        Task<IList<SkuInfo>> GetDetalleSku(string sku);
        Task<bool> AsignLineToSKU(string sku, int linie, string userId, int turno, decimal kgxhr);
        Task<bool> UpdateSkuById(string id, int  MultiploCant, string MultiploCat, float Variable, string itemName, float minWeight, float flejeCant, float anilloCant, float maderaCant, float arpillaCant, float costalesCant, float tarimaCant, int planta, string usuario);
        IList<SkuInfo> GetBySku(string sku, string planta);
    }
}
