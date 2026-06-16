using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.TarasRepo;

namespace WebApiPtmHana.BLL.Services.TarasServ
{
    public class TarasService : ITaras
    {
        private readonly IGenericRepoTaras _taras;

        public TarasService(IGenericRepoTaras taras)
        {
            _taras = taras;
        }
        public async Task<bool> UpdateTaras(string jsonTaras)
        {
            return await _taras.UpdateTaras(jsonTaras);
        }
    }
}
