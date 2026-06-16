using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.DAL.DataContext.Repositories.UserTokensRepo;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.UserTokenServ
{
    public class UserTokenService : IUserTokens
    {
        private readonly IGenericRepoUserTokens<UserTokens> _repositorie;
        public UserTokenService(IGenericRepoUserTokens<UserTokens> repositorie) 
        {
            _repositorie = repositorie;
        }
        public Task<bool> AddJwtToUser(int empleadoId, string LoginProvider, string Name, string Value)
        {
            return _repositorie.AddJwtToUser(empleadoId, LoginProvider, Name, Value);
        }

        public Task<List<UserTokens>> GetUserTokenById(int empleadoId)
        {
            return _repositorie.GetJwtById(empleadoId);
        }

        public Task<bool> LogOutUser(int empleadoId)
        {
            return _repositorie.LogOutUser(empleadoId);
        }
    }
}
