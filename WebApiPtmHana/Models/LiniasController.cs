using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WebApiPtmHana.BLL.Services.BitacoraLServ;
using WebApiPtmHana.BLL.Services.LineasServ;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiniasController : ControllerBase
    {
        private readonly IBitacoraLinia _bitacora;
        private readonly ILinia _lineas;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly string connectionstring;

        public LiniasController(IBitacoraLinia bitacora, ILinia lineas, IConfiguration configuration, ApplicationDbContext context)
        {
            _bitacora = bitacora;
            _lineas = lineas;
            _configuration = configuration;
            _context = context;
            connectionstring = configuration.GetConnectionString("SQLConnection");
        }

        [HttpGet]
        [Route("GetBitacoraLinie")]
        public async Task<IActionResult> GetBitacoraLinie([FromHeader] string linie, [FromHeader] string planta)
        {
            try
            {
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
        [Route("AsignSkuToLinie")]
        public async Task<IActionResult> AsignSkuToLinie([FromHeader] string sku, [FromHeader] string linea, 
            [FromHeader] string username, [FromHeader] string turno, [FromHeader] string kgxhr, [FromHeader] string userId, 
            [FromHeader] string ordenFabricacion, [FromHeader] string folio, [FromHeader] string planta)
        {
            try
            {
                if (!sku.IsNullOrEmpty() || !linea.IsNullOrEmpty())
                {
                    int linia = int.Parse(linea);
                    int turnoOp = int.Parse(turno);
                    string kghr = kgxhr;//.Replace(".", ",");
                    float KgXhr = float.Parse(kghr);
                    using(SqlConnection conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();
                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = conn;
                            command.CommandText = "Sppdx_AsignarLineas";
                            command.Parameters.AddWithValue("@codigoitem",sku);
                            command.Parameters.AddWithValue("@idlinea", linea);
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@kghrprod", KgXhr);
                            command.Parameters.AddWithValue("@ordenfabri", ordenFabricacion);
                            command.Parameters.AddWithValue("@folio", folio);
                            command.Parameters.AddWithValue("@planta", int.Parse(planta));
                            command.CommandType = CommandType.StoredProcedure;

                            command.ExecuteNonQuery();

                            return StatusCode(StatusCodes.Status200OK, new { response = $"EL sku {sku} a sido asignada a la linea {linea}" });
                        }
                    }
                    //bool response = await _lineas.AsignSkuToLinie(sku, linia, username, turnoOp, KgXhr);
                    //if (response)
                    //{
                    //}
                    //else
                    //{
                    //    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Algo paso dentro de los servicios" });
                    //}
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Los parametro no se mandan correctamente" });
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateNewLinie")]
        public async Task<IActionResult> CreateNewLinie([FromHeader] string idLinea, [FromHeader] string proceso, [FromHeader] string planta)
        {
            try
            {
                bool response = await _lineas.CreateNewLinie(int.Parse(idLinea), proceso, planta);
                if (!response)
                    return StatusCode(StatusCodes.Status404NotFound, new { response = "Se han encontrado errores al registrar la línea" });

                return StatusCode(StatusCodes.Status201Created, new { response = "Se ha creado la línea correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Procedure para eliminar una linea de produccion ya existente
        /// </summary>
        /// <param name="Linea"></param>
        /// <param name="planta"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteLinea")]
        public async Task<IActionResult> DeleteLinea([FromHeader] string Linea, [FromHeader] string planta)
        {
            try
            {
                string JSONstringSp = string.Empty;

                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_DeleteLineaProcess";
                        command.Connection = conn;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", int.Parse(planta));
                        command.Parameters.AddWithValue("@Linea", int.Parse(Linea));

                        command.ExecuteNonQuery();
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Linea Eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("ValidateSkuByLinie")]
        public async Task<IActionResult> ValidateSkuByLinie([FromHeader] string idLinea, [FromHeader] string itemcode, [FromHeader] string planta)
        {
            try
            {
                var lineaSku = _context.Lineas.Where(x => x.NumLinea == int.Parse(idLinea) && x.Planta == int.Parse(planta)).ToList();
                if (itemcode == lineaSku[0].ItemCodeLinea)
                {
                    return StatusCode(StatusCodes.Status200OK, new { response = "Sku correctos" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { response = "Skus no coinciden por linea" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("UnassignLinieAndTicket")]
        public async Task<IActionResult> UnassignLinieAndTicket([FromHeader] string idLinea, [FromHeader] string userId, [FromHeader] string planta)
        {
            try
            {
                bool response = await _lineas.UnassignLinieAndTicket(int.Parse(idLinea), planta);

                if (!response)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Error al desasignar la linea" });

                return StatusCode(StatusCodes.Status200OK, new { response = $"Linea {idLinea} desasignada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }
}
