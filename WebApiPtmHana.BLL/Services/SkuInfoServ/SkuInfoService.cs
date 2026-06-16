using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.SkuInfos;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.SkuInfoServ
{
    public class SkuInfoService : ISkuInfo
    {
        private readonly IGenericRepoSkuInfo<SkuInfo> _repositorie;
        public SkuInfoService(IGenericRepoSkuInfo<SkuInfo> repositorie)
        {
            _repositorie = repositorie;
        }
        public async Task<bool> AsignLineToSKU(string sku, int linie, string userId, int turno, decimal kgxhr)
        {
            return await _repositorie.AsignSkuToLine(sku, linie, userId, turno, kgxhr);
        }

        public async Task<IList<SkuInfo>> GetAllSkuInfos(int planta)
        {
            return _repositorie.GetAll(planta);
        }

        public IList<SkuInfo> GetBySku(string sku, string planta)
        {
            return _repositorie.GetBySku(sku, planta);
        }

        public async Task<IList<SkuInfo>> GetDetalleSku(string sku)
        {
            return await _repositorie.ValidSkuInfos(int.Parse(sku));
        }

        public async Task<bool> UpdateSkuById(string id, int  MultiploCant, string MultiploCat, float Variable, string itemName, float minWeight, float flejeCant, float anilloCant, float maderaCant, float arpillaCant, float costalesCant, float tarimaCant, int planta, string usuario)
        {
            return await _repositorie.UpdateById(id, MultiploCant, MultiploCat, Variable, itemName, minWeight, flejeCant, anilloCant, maderaCant, arpillaCant, costalesCant, tarimaCant, planta, usuario);
        }
    }
}
