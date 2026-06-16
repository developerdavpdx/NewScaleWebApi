using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.TicketsInfoServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.FolioModel;
using WebApiPtmHana.Models.InfoTickets;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoTicketsController : ControllerBase
    {
        private readonly IInfoTickets _infoTickets;
        private readonly IConfiguration _configuration;
        //private readonly UserManager<UsersIdentity> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly string connectionString;
        //TokensService _tokenService = new TokensService();
        private readonly string prefijo;

        public InfoTicketsController(IInfoTickets infoTickets, IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            //_userManager = userManager;
            _infoTickets = infoTickets;
            _context = context;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            //Llamamos al prefijo para los stores
            prefijo = configuration["StoredProcedurePrefix"];
        }

        [HttpGet]
        [Route("GetTickets")]
        public async Task<IActionResult> GetAllTickets([FromHeader] string numplanta)
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

                int planta = int.Parse(numplanta);

                IList<Datos.TicketsInfo> TicketsInfos = _infoTickets.GetTickets(planta);
                if (TicketsInfos.Count == 0)
                    return StatusCode(StatusCodes.Status200OK, new { response = "No se encuentran tickets en este momento" });

                return StatusCode(StatusCodes.Status200OK, new { TicketsInfos });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("GetTicketByLinea")]
        public async Task<IActionResult> GetTicketByLinea([FromHeader] string linea, [FromHeader] string userId)
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

                IList<Datos.TicketsInfo> TicketsInfos = _infoTickets.GetTicketByLinie(int.Parse(linea));

                if (TicketsInfos.Count == 0)
                    return StatusCode(StatusCodes.Status200OK, new { response = $"No se encuentran tickets en este momento en la línea {linea}" });

                return StatusCode(StatusCodes.Status200OK, new { TicketsInfos });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetTicketById")]
        public async Task<IActionResult> GetTicketById([FromHeader] string id)
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

                IList<TicketsInfo> TicketsInfos = _infoTickets.GetTicketById(int.Parse(id));

                if (TicketsInfos.Count == 0)
                    return StatusCode(StatusCodes.Status200OK, new { response = $"No se encuentran tickets en este momento con el id {id}" });

                return StatusCode(StatusCodes.Status200OK, new { TicketsInfos });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetPrintTicketById")]
        public async Task<IActionResult> GetPrintTicketById([FromHeader] string id)
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

                IList<TicketsInfo> TicketsInfos = _infoTickets.GetPrintTickeyById(int.Parse(id));

                if (TicketsInfos.Count == 0)
                    return StatusCode(StatusCodes.Status200OK, new { response = $"No se encuentran tickets en este momento con el id {id}" });

                return StatusCode(StatusCodes.Status200OK, new { TicketsInfos });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        //obtenemos la estructura html del ticket a imprimir (pesaje pt y reimpresiones)
        [HttpGet]
        [Route("getTicketTemplate")]
        public async Task<IActionResult> getTicketTemplate()
        {
            string filepath = Directory.GetCurrentDirectory() + "/TicketPTM/TicketPtM.html";
            StreamReader str = new(filepath);
            string txt = str.ReadToEnd();
            str.Close();
            return StatusCode(StatusCodes.Status200OK, new { response = txt });
        }

        [HttpGet]
        [Route("getTicketTemplateMolinos")]
        public async Task<IActionResult> getTicketTemplateMolinos()
        {
            string filepath = Directory.GetCurrentDirectory() + "/TicketPTM/TicketMolinos.html";
            StreamReader str = new(filepath);
            string txt = str.ReadToEnd();
            str.Close();
            return StatusCode(StatusCodes.Status200OK, new { response = txt });
        }

        //obtenemos la estructura html del ticket a imprimir (envioScrap)
        [HttpGet]
        [Route("getTicketTemplateScrap")]
        public async Task<IActionResult> getTicketTemplateScrap()
        {
            string filepath = Directory.GetCurrentDirectory() + "/TicketPTM/TicketScrap.html";
            StreamReader str = new(filepath);
            string txt = str.ReadToEnd();
            str.Close();
            return StatusCode(StatusCodes.Status200OK, new { response = txt });
        }

        [HttpPost]
        [Route("UpdateTicketById")]
        public async Task<IActionResult> UpdateTicketById([FromHeader] string idTicket, [FromHeader] string peso, [FromHeader] string pesoBascula, [FromHeader] string porcentaje, [FromHeader] string calidad, 
            [FromHeader] string pesotubo, [FromHeader] string? tubos, [FromHeader] string planta)
        {
            try
            {
                //peso = peso.Replace(".", ",");
                //porcentaje = porcentaje.Replace(".", ",");
                //pesotubo = pesotubo.Replace(".", ",");
                bool Calidad = calidad == "1" ? true : false;
                bool response = await _infoTickets.UpdateTicket(int.Parse(idTicket), float.Parse(peso), float.Parse(pesoBascula), float.Parse(porcentaje), Calidad, 
                    decimal.Parse(pesotubo), tubos, int.Parse(planta));
                if (!response)
                {
                    var ticket = await _context.TicketsInfos.FindAsync(int.Parse(idTicket));
                    _context.TicketsInfos.Remove(ticket);
                    _context.SaveChanges();
                    return StatusCode(StatusCodes.Status404NotFound, new { response = "Error al actualizar el registro del ticket" });
                } else
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = "Se han actualizado los campos correctamente" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
        //PASO 2 ESCANEO CREAR TICKET
        [HttpPost]
        [Route("AddNewTicket")]
        public async Task<IActionResult> AddNewTicket([FromHeader] string linea, [FromHeader] string sku, [FromHeader] string turno, [FromHeader] string planta)
        {
            try
            {
                var skuInfo = _context.SkuInfos.Where(x => x.ItemCode == sku).ToList();
                var idTicket = _context.TicketsInfos.Max(x => x.IdTicket);
                //var skuInfos = _context.SkuInfos.Where(x => x.ItemCode == sku).ToList();

                List<FolioModel> JsonOwor = new List<FolioModel>();
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetOrdenFabricacion";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("ItemCOde", HanaDbType.NVarChar).Value = sku;
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
                                    JsonOwor = JsonConvert.DeserializeObject<List<FolioModel>>(JSONString);
                                }
                            }
                        }
                    }
                }

                var lineaL = _context.Lineas.Where(x => x.NumLinea == int.Parse(linea) && x.Planta == int.Parse(planta)).ToList();

                TicketsInfo ticket = new TicketsInfo
                {
                    Folio = JsonOwor[0].FOLIO.ToString(),
                    Categoria = "Producto Terminado",
                    Id_Linea = int.Parse(linea),
                    ItemCode = sku,
                    OrdenFabricacion = lineaL[0].OrdenFabricacion,
                    ItemName = skuInfo[0].ItemName,
                    Turno = int.Parse(turno),
                    Operacion = idTicket == null ? 1 : idTicket + 1,
                    AsignadoTicket = false,

                };

                await _context.TicketsInfos.AddAsync(ticket);
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status200OK, new { response = ticket.IdTicket });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("FilterTickets")]
        public IActionResult FilterTickets([FromHeader] string? folio, [FromHeader] string? turno, [FromHeader] string? fechaInicio, [FromHeader] string? fechaFin, [FromHeader] string planta)
        {
            try
            {
                int numplanta = int.Parse(planta);
                var tickets = _infoTickets.FilterTickest(turno == null ? null : int.Parse(turno), folio, fechaInicio, fechaFin, numplanta);
                
                return StatusCode(StatusCodes.Status200OK, new { tickets });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }
}
