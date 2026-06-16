using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;
using Newtonsoft.Json;

namespace WebApiPtmHana.DAL.DataContext.Repositories.TarasRepo
{
    public class TarasRepositorie : IGenericRepoTaras
    {
        private readonly ApplicationDbContext _context;
        public TarasRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateTaras(string jsonTaras)
        {
            try
            {
                var taras = JsonConvert.DeserializeObject<string[,]>(jsonTaras);
                //verificamos que taras no venga en null, para evitar operar una matriz nula
                if (taras != null)
                {
                    for (int i = 0; i < taras.GetLength(0); i++)
                    {
                        //buscamos el sku que viene del excel y coincida con ItemCode
                        var sku = _context.SkuInfos.Where(x => x.ItemCode == taras[i, 0].ToString()).ToList();

                        //si el sku ya existe en la tabla, lo actualizamos
                        if (sku.Count > 0)
                        {
                            var infoSku = await _context.SkuInfos.FindAsync(sku[0].Id);
                            string tara = taras[i, 1].ToString();
                            //tara = tara.Replace(".", ",");
                            infoSku.Variable = float.Parse(tara);
                            infoSku.LastUpdate = DateTime.Now;
                            _context.Update(infoSku);
                            await _context.SaveChangesAsync();
                        }
                        //si no existe, lo incertamos en la tabla
                        else
                        {
                            string tara = taras[i, 1].ToString();
                            //tara = tara.Replace(".", ",");
                            SkuInfo skuInfo = new SkuInfo()
                            {
                                ItemCode = taras[i, 0].ToString(),
                                ItemName = taras[i, 2].ToString(),
                                Variable = float.Parse(tara),
                                LastUpdate = DateTime.Now
                            };

                            await _context.SkuInfos.AddAsync(skuInfo);
                            await _context.SaveChangesAsync();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return false;
            }
        }
    }
}
