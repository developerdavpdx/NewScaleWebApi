using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.UserTokensRepo
{
    public class UserTokensRepositorie : IGenericRepoUserTokens<UserTokens>
    {
        private readonly ApplicationDbContext _context;
        public UserTokensRepositorie(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<bool> AddJwtToUser(int empleadoId, string LoginProvider, string Name, string Value)
        {
            try
            {
                var userExiste = _context.UserTokens.Where(x => x.IdEmpleado == empleadoId).ToList();
                if(userExiste.Count == 0)
                {
                    var userTokens = new UserTokens()
                    {
                        IdEmpleado = empleadoId,
                        LoginProvider = LoginProvider,
                        Name = Name,
                        Value = Value
                    };

                    await _context.UserTokens.AddAsync(userTokens);
                    await _context.SaveChangesAsync();

                    return true;
                } else
                {
                    var userToken = _context.UserTokens.Find(userExiste[0].Id);
                    
                    userToken.Value = Value;

                    _context.Update(userToken);
                    await _context.SaveChangesAsync();

                    return true;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<UserTokens>> GetJwtById(int empleadoId)
        {
            List<UserTokens> userTokens= _context.UserTokens.Where(x => x.IdEmpleado == empleadoId).ToList();

            if(userTokens.Count >= 0) {
                return userTokens;
            } else
            {
                return userTokens;
            }
        }

        public async Task<bool> LogOutUser(int empleadoId)
        {
            List<UserTokens> userTokens = _context.UserTokens.Where(x => x.IdEmpleado== empleadoId).ToList();

            if(userTokens.Count>0)
            {
                var user = _context.UserTokens.Find(userTokens[0].Id);
                _context.UserTokens.Remove(user);
                return true;
            } else
            {
                return false;
            }
        }
    }
}
