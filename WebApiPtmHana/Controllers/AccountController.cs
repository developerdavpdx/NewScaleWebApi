using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.UserTokenServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.AccountModel;
using WebApiPTMTest.Models;
using WebApiPTMTest.Services;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly HanaConnection _hanaconnection;
        private readonly IConfiguration _configuration;
        TokensService generateToken = new TokensService();
        private readonly string connectionString;
        private readonly IUserTokens _userToken;
        private readonly string prefijo;


        public AccountController(HanaConnection hanaConnection, IConfiguration configuration, IUserTokens userTokens)
        {
            _hanaconnection = hanaConnection;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            _userToken = userTokens;
            _configuration = configuration;
            //Llamamos al prefijo para los stores
            prefijo = configuration["StoredProcedurePrefix"];
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_LOGIN";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("Email", HanaDbType.NVarChar).Value = login.UserEmail;
                        command.Parameters.Add("Pass", HanaDbType.NVarChar).Value = login.Password;
                        using (HanaDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                hana.Items = new DataTable();
                                hana.Items.Load(reader);
                                using (hana.Items)
                                {
                                    hana.response = new Dictionary<string, string>();
                                    string JSONString = string.Empty;
                                    JSONString = JsonConvert.SerializeObject(hana.Items);
                                    var JsonAccount = JsonConvert.DeserializeObject<List<LoginResultModel>>(JSONString);
                                    if (JsonAccount[0].STATUS == "200")
                                    {
                                        var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                                        var token = generateToken.GenerateTokenAccess(JsonAccount[0].NOMBRECOMPLETO, login.UserEmail, jwtSettings.Key, jwtSettings.ExpiresInMinutes, jwtSettings.Audience, jwtSettings.Issuer);
                                        if(token == null)
                                            return StatusCode(StatusCodes.Status404NotFound, new { response = "Error en el proceso de autenticación" });

                                        bool result = await _userToken.AddJwtToUser(JsonAccount[0].EMPLEADOID, "JWT", "Access-Token", token.ToString());

                                        if(!result)
                                            return StatusCode(StatusCodes.Status404NotFound, new { response = "Error en el proceso de autenticación" });

                                        return StatusCode(StatusCodes.Status200OK, new { data = JsonAccount }); // Devolvemos el resultado en JSON
                                    } else
                                    {
                                        return StatusCode(StatusCodes.Status404NotFound, new { response = "Credenciales invalidas" });
                                    }
                                }
                            }
                            else
                            {
                                hana.response = new Dictionary<string, string>();
                                return StatusCode(StatusCodes.Status404NotFound, new { response = "No se encuentran datos" });
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut([FromHeader] string userId)
        {
            try
            {
                bool existe = await _userToken.LogOutUser(int.Parse(userId));
                if (existe)
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = "Se ha cerrado la sesión" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existen datos de autenticacion" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("ValidateToken")]
        public async Task<IActionResult> ValidateToken([FromHeader] string userId)
        {
            try
            {
                List<UserTokens> userToken = await _userToken.GetUserTokenById(int.Parse(userId));

                if(userToken == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El usuario no esta autenticado" });

                var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                bool checkToken = generateToken.ValidateTokenAccess(userToken[0].Value, jwtSettings.Key, jwtSettings.Audience, jwtSettings.Issuer);

                if (checkToken)
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = "El usuario esta autenticado" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El usuario no esta autenticado" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }
}
