using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.SkuInfos
{
    public interface IGenericRepoSkuInfo<TEntityModel> where TEntityModel : class
    {
        IList<TEntityModel> GetAll(int planta);
        IList<TEntityModel> GetBySku(string sku, string planta);
        Task<bool> AsignSkuToLine(string sku, int line, string userId, int turno, decimal kgxhr);
        Task<bool> UpdateById(string id, int  MultiploCant, string MultiploCat, float Variable, string itemName, float minWeight, float flejeCant, float anilloCant, float maderaCant, float arpillaCant, float costalesCant, float tarimaCant, int planta, string usuario);
        Task<IList<SkuInfo>> ValidSkuInfos(int sku);
    }
}
