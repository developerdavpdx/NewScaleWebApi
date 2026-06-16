using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.UserTokensRepo
{
    public interface IGenericRepoUserTokens<TEntityModel> where TEntityModel : class
    {
        Task<bool> AddJwtToUser(int empleadoId,string LoginProvider, string Name, string Value);
        Task<List<UserTokens>> GetJwtById(int empleadoId);
        Task<bool> LogOutUser(int empleadoId);
    }
}
