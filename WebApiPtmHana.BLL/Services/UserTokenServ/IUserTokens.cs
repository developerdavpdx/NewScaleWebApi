using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.BLL.Services.UserTokenServ
{
    public interface IUserTokens
    {
        Task<bool> AddJwtToUser(int empleadoId, string LoginProvider, string Name, string Value);
        Task<List<UserTokens>> GetUserTokenById(int empleadoId);
        Task<bool> LogOutUser(int empleadoId);
    }
}
