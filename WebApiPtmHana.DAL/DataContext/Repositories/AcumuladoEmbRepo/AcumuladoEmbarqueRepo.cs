using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.AcumuladoEmbRepo
{
    public class AcumuladoEmbarqueRepo : IGenericRepoAcumuladoEmb<AcumuladoEmbarque>
    {
        private readonly ApplicationDbContext _context;

        public AcumuladoEmbarqueRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AgregarAcomuladoEmbarque(string proceso, string familia, float acomulado, int planta)
        {
            AcumuladoEmbarque acumuladoembarque = new AcumuladoEmbarque()
            {
                Proceso = proceso,
                Familia = familia,
                Acumulado = acomulado,
                FechaIngreso = DateTime.Now,
                Planta = planta
            };

            await _context.AddAsync(acumuladoembarque);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
