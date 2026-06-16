using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.SkuInfos
{
    public class SkuInfoRepositorie : IGenericRepoSkuInfo<SkuInfo>
    {
        private readonly ApplicationDbContext _context;
        public SkuInfoRepositorie(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<bool> AsignSkuToLine(string sku, int line, string userId, int turno, decimal kgxhr)
        {
            try
            {
                if (!sku.IsNullOrEmpty() && line != 0)
                {
                    int idSKU = int.Parse(sku);
                    IList<SkuInfo> skuInfos = _context.SkuInfos.Where(x => x.Id == idSKU).ToList();
                    if (skuInfos.Count > 0)
                    {
                        var linea = await _context.Lineas.FindAsync(line);

                        if (linea == null)
                            return false;

                        if (linea.Asignado == true)
                            return false;

                        linea.Asignado = true;
                        linea.KgXHr = kgxhr;
                        linea.ItemCodeLinea = skuInfos[0].ItemCode;
                        await _context.SaveChangesAsync();

                        BitacoraAsignacionLinia bitacora = new BitacoraAsignacionLinia()
                        {
                            Linea = line,
                            FechaAsignacion = DateTime.Now,
                            Trabajador = userId,
                            CodigoItem = skuInfos[0].ItemCode
                        };

                        var bitresponse = await _context.AddAsync(bitacora);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public IList<SkuInfo> GetAll(int planta)
        {
            List<SkuInfo> skuInfo = new List<SkuInfo>();
            if (planta == 0)
            {
                skuInfo = _context.SkuInfos.ToList();
            } else
            {
                skuInfo = _context.SkuInfos.Where(x => x.Planta == planta).ToList();
            }
            return skuInfo;
        }

        public IList<SkuInfo> GetBySku(string sku, string planta)
        {
            List<SkuInfo> skuInfo = _context.SkuInfos.Where(x => x.ItemCode == sku && x.Planta == int.Parse(planta)).ToList();
            return skuInfo;
        }

        public async Task<bool> UpdateById(string id, int  MultiploCant, string MultiploCat, float Variable, string itemName, 
            float minWeight, float flejeCant, float anilloCant, float maderaCant, float arpillaCant, float costalesCant, float tarimaCant, int planta, string usuario)
        {
            try
            {
                List<SkuInfo> skuInfo = _context.SkuInfos.Where(x => x.ItemCode == id && x.Planta == planta).ToList();

                if(skuInfo.Count == 0)
                {
                    SkuInfo sku = new SkuInfo()
                    {
                        ItemCode = id,
                        LastUpdate = DateTime.Now,
                        MultiploCant = MultiploCant,
                        MultiploCategoria = MultiploCat,
                        Variable = Variable,
                        Proceso = "PT",
                        ItemName = itemName,
                        MinWeight = minWeight,
                        AnilloCant = anilloCant,
                        ArpillaCant = arpillaCant,
                        CostalesCant = costalesCant,
                        FlejeCant = flejeCant,
                        MaderaCant = maderaCant,
                        TarimaCant = tarimaCant,
                        Planta = planta,
                        Usuario = usuario
                    };

                    await _context.SkuInfos.AddAsync(sku);
                    await _context.SaveChangesAsync();

                    return true;
                } else
                {
                    SkuInfo skuinfo = _context.SkuInfos.Find(skuInfo[0].Id);

                    
                    skuinfo.LastUpdate = DateTime.Now;
                    skuinfo.Variable = Variable;
                    skuinfo.MultiploCant = MultiploCant;
                    skuinfo.MultiploCategoria = MultiploCat;
                    skuinfo.MinWeight = minWeight;
                    skuinfo.ArpillaCant = arpillaCant;
                    skuinfo.AnilloCant = anilloCant;
                    skuinfo.MaderaCant = maderaCant;
                    skuinfo.CostalesCant = costalesCant;
                    skuinfo.TarimaCant = tarimaCant;
                    skuinfo.FlejeCant = flejeCant;
                    skuinfo.Usuario = usuario;

                    _context.Update(skuinfo);
                    await _context.SaveChangesAsync();

                    return true;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<IList<SkuInfo>> ValidSkuInfos(int sku)
        {
            IList<SkuInfo> skuInfos = _context.SkuInfos.Where(x => x.Id == sku).ToList();
            return skuInfos;
        }
    }
}
