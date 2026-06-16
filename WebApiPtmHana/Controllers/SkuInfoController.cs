using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.BitacoraLServ;
using WebApiPtmHana.BLL.Services.SkuInfoServ;
using WebApiPtmHana.BLL.Services.UserTokenServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.WeightCheck;
using WebApiPtmHana.Services;
using WebApiPTMTest.Models;
using WebApiPTMTest.Services;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkuInfoController : ControllerBase
    {
        private readonly ISkuInfo _skuinfoService;
        private readonly IBitacoraLinia _bitacora;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        TokensService _tokenService = new TokensService();
        private readonly string connectionString;
        private readonly IUserTokens _userTokens;
        private readonly IHttpContextAccessor _accesor;
        private readonly ITempDataDictionaryFactory _tempData;

        public SkuInfoController(ISkuInfo skuinfoService, IBitacoraLinia bitacora, IConfiguration configuration,
            ApplicationDbContext context, IUserTokens userTokens, IHttpContextAccessor accesor, ITempDataDictionaryFactory tempData)
        {
            _skuinfoService = skuinfoService;
            _bitacora = bitacora;
            _configuration = configuration;
            _context = context;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            _userTokens = userTokens;
            _accesor = accesor;
            _tempData = tempData;
        }

        [HttpGet]
        [Route("GetAllSkuInfo")]
        public async Task<IActionResult> GetAllSkuInfo([FromHeader] string userId, [FromHeader] string planta)
        {
            try
            {
                //var user = await _userManager.FindByIdAsync(userId);
                //if (user == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe el usuario dentro del sistema" });

                //var token = await _userManager.GetAuthenticationTokenAsync(user!, "JWT", "Access-Token");
                //if (token == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe token de usuario Iniciar Sesión" });

                //var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                //bool checkToken = _tokenService.ValidateTokenAccess(token!, jwtSettings.Key, jwtSettings.Audience, jwtSettings.Issuer);
                //if (!checkToken)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El token a expirado. Favor de iniciar sesión de nueva cuenta" });
                List<UserTokens> userTokens = await _userTokens.GetUserTokenById(int.Parse(userId));
                if (userTokens.Count == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe autenticación del usuario" });

                var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                bool checkToken = _tokenService.ValidateTokenAccess(userTokens[0].Value, jwtSettings.Key, jwtSettings.Audience, jwtSettings.Issuer);


                IList<SkuInfo> skuInfos = await _skuinfoService.GetAllSkuInfos(int.Parse(planta));

                if (skuInfos.Count > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { skuInfos });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No se encuentra información de los SKU" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("SkuInfo")]
        public async Task<IActionResult> GetSkuInfo([FromHeader] string sku, [FromHeader] string userId)
        {
            try
            {
                //var user = await _userManager.FindByIdAsync(userId);
                //if (user == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe el usuario dentro del sistema" });

                //var token = await _userManager.GetAuthenticationTokenAsync(user!, "JWT", "Access-Token");
                //if (token == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe token de usuario Iniciar Sesión" });

                //var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                //bool checkToken = _tokenService.ValidateTokenAccess(token!, jwtSettings.Key, jwtSettings.Audience, jwtSettings.Issuer);
                //if (!checkToken)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El token a expirado. Favor de iniciar sesión de nueva cuenta" });

                if (!sku.IsNullOrEmpty())
                {
                    IList<SkuInfo> skuinfo = await _skuinfoService.GetDetalleSku(sku);
                    if (skuinfo.Count > 0)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { skuinfo });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El Sku no existe dentro de los registros" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El parametro viene nulo o vacío" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("AsignSkuToLinie")]
        public async Task<IActionResult> AsignSkuToLinie([FromHeader] string sku, [FromHeader] string linea, [FromHeader] string userId, [FromHeader] string turno, [FromHeader] string kgxhr)
        {
            try
            {

                //var user = await _userManager.FindByIdAsync(userId);
                //if (user == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe el usuario dentro del sistema" });

                //var token = await _userManager.GetAuthenticationTokenAsync(user!, "JWT", "Access-Token");
                //if (token == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe token de usuario Iniciar Sesión" });

                //var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                //bool checkToken = _tokenService.ValidateTokenAccess(token!, jwtSettings.Key, jwtSettings.Audience, jwtSettings.Issuer);
                //if (!checkToken)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El token a expirado. Favor de iniciar sesión de nueva cuenta" });

                if (!sku.IsNullOrEmpty() || !linea.IsNullOrEmpty())
                {
                    int linia = int.Parse(linea);
                    int turnoOp = int.Parse(turno);
                    decimal Kgxhr = decimal.Parse(kgxhr);
                    bool response = await _skuinfoService.AsignLineToSKU(sku, linia, userId, turnoOp, Kgxhr);
                    if (response)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { response = $"EL sku {sku} a sido asignada a la linea {linea}" });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Algo paso dentro de los servicios" });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Los parametro no se mandan correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetBitacoraLinie")]
        public async Task<IActionResult> GetBitacoraLinie([FromHeader] string linie, [FromHeader] string userId, [FromHeader] string planta)
        {
            try
            {
                //var user = await _userManager.FindByIdAsync(userId);
                //if (user == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe el usuario dentro del sistema" });

                //var token = await _userManager.GetAuthenticationTokenAsync(user!, "JWT", "Access-Token");
                //if (token == null)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "No existe token de usuario Iniciar Sesión" });

                //var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
                //bool checkToken = _tokenService.ValidateTokenAccess(token!, jwtSettings.Key, jwtSettings.Audience, jwtSettings.Issuer);
                //if (!checkToken)
                //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "El token a expirado. Favor de iniciar sesión de nueva cuenta" });

                IList<BitacoraAsignacionLinia> bitacora = _bitacora.GetBitAsignByLinie(int.Parse(linie), planta);
                if (bitacora.Count > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, new { bitacora });
                }
                else
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = $"No hay bitacora de la línea número {linie}" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("ValidateWeight")]
        public async Task<IActionResult> ValidateWeight([FromHeader] string ItemCode, [FromHeader] string? numTubos)
        {
            try
            {
                //AQUI VOY EL VALIDAR QUE NO ESTE EN 0
                
                int idBascula = 1;
                List<EstadosPesos> existePeso = _context.EstadosPesos.Where(x => x.Id_Bascula == idBascula).ToList();
                if (existePeso.Count == 0)
                    return StatusCode(StatusCodes.Status404NotFound, new { response = "No existe peso en la tabla" });

                //existePeso[0].PesoBascula = 500;

                if (existePeso[0].PesoBascula == 0)
                    return StatusCode(StatusCodes.Status404NotFound, new { response = "No existe peso registrado o esta en 0" });

                int tubosTotales = 0;

                List<SkuInfo> skuInfos = _context.SkuInfos.Where(x => x.ItemCode == ItemCode).ToList();
                if(numTubos == "" || numTubos == null)
                {
                    tubosTotales = Convert.ToInt32(numTubos);
                } else
                {
                    tubosTotales = Convert.ToInt32(skuInfos[0].MultiploCant);
                }
                float peso = float.Parse(existePeso[0].PesoBascula.ToString());
                float? pesoNeto = peso - (skuInfos[0].Variable + skuInfos[0].AnilloCant + skuInfos[0].FlejeCant + skuInfos[0].ArpillaCant + skuInfos[0].TarimaCant + skuInfos[0].MaderaCant + skuInfos[0].CostalesCant);
                float? pesoEstandar = tubosTotales * skuInfos[0].MinWeight;
                float? pesoTubos =  pesoNeto / tubosTotales;
                float? sobrePeso = ((pesoNeto / pesoEstandar) - 1) * 100;
                CheckOverWeight _overWeight = new CheckOverWeight();
                string proceso = skuInfos[0].ItemName.ToLower().Contains("pvc") ? "pvc" : "iny";
                int result = _overWeight.OverWeight(sobrePeso, proceso);

                List<EstadosSemaforo> listsemaforo = _context.EstadosSemaforos.Where(x => x.Id_Bascula == idBascula).ToList();
                if (listsemaforo.Count == 0)
                {
                    EstadosSemaforo semaforo = new EstadosSemaforo()
                    {
                        Id_Bascula = idBascula,
                        Estado = result
                    };
                    await _context.EstadosSemaforos.AddAsync(semaforo);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var semaforo = _context.EstadosSemaforos.Find(listsemaforo[0].Id);
                    semaforo.Estado = result;
                    semaforo.Id_Bascula = idBascula;

                    _context.Update(semaforo);
                    await _context.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status200OK, new { Result = result, PesoNeto = pesoNeto, SobrePeso = sobrePeso, PesoTubos = pesoTubos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }


        //PASO 4 ESCANEO DESPUES DE OBTENER EL PESO DEL TXT
        [HttpPost]
        [Route("ValidateWeight2")]
        public async Task<IActionResult> ValidateWeight2([FromHeader] string ItemCode, [FromHeader] string peso, [FromHeader] string? numTubos, [FromHeader] string planta)
        {
            try
            {
                //AQUI VOY EL VALIDAR QUE NO ESTE EN 0
                peso = peso.Replace(",", ".");
                int idBascula = 1;
                int tubosTotales = 0;
                // si el num de tubos esta vacio o es null, lo busca en la tabla skuInfos
                List<SkuInfo> skuInfos = _context.SkuInfos.Where(x => x.ItemCode == ItemCode && x.Planta == int.Parse(planta)).ToList();
                if (numTubos == "" || numTubos == null)
                {
                    tubosTotales = Convert.ToInt32(skuInfos[0].MultiploCant);
                }
                else
                {
                    tubosTotales = Convert.ToInt32(numTubos);
                }

                float pesoB = float.Parse(peso);
                float? pesoNeto = pesoB - (skuInfos[0].Variable + skuInfos[0].AnilloCant + skuInfos[0].FlejeCant + skuInfos[0].ArpillaCant 
                    + skuInfos[0].TarimaCant + skuInfos[0].MaderaCant + skuInfos[0].CostalesCant);
                float? pesoEstandar = tubosTotales * skuInfos[0].MinWeight;     //peso estandar unitario
                float? pesoEstandarT = tubosTotales * pesoEstandar;     //peso estandar total
                float? pesoTubos = pesoNeto / tubosTotales;
                float? sobrePeso = ((pesoNeto / pesoEstandar) - 1) * 100;
                CheckOverWeight _overWeight = new CheckOverWeight();
                string proceso = skuInfos[0].ItemName.ToLower().Contains("pvc") ? "pvc" : "iny";
                //Determinar si es sobre o bajo peso o esta ok
                int result = _overWeight.OverWeight(sobrePeso, proceso);

                List<EstadosSemaforo> listsemaforo = _context.EstadosSemaforos.Where(x => x.Id_Bascula == idBascula).ToList();

                if (listsemaforo.Count == 0)
                {
                    EstadosSemaforo semaforo = new EstadosSemaforo()
                    {
                        Id_Bascula = idBascula,
                        Estado = result
                    };
                    await _context.EstadosSemaforos.AddAsync(semaforo);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var semaforo = _context.EstadosSemaforos.Find(listsemaforo[0].Id);
                    semaforo.Estado = result;
                    semaforo.Id_Bascula = idBascula;

                    _context.Update(semaforo);
                    await _context.SaveChangesAsync();
                }

                return StatusCode(StatusCodes.Status200OK, new { Result = result, PesoNeto = pesoNeto, SobrePeso = sobrePeso, PesoTubos = pesoTubos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
        }
    }
}
