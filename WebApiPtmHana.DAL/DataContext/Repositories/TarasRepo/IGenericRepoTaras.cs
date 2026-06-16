using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPtmHana.DAL.DataContext.Repositories.TarasRepo
{
    public interface IGenericRepoTaras
    {
        Task<bool> UpdateTaras(string jsonTaras);
    }
}
