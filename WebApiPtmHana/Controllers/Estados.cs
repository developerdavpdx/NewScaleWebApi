using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using WebApiPtmHana.DAL.DataContext;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models.Estados;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Estados : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionstring;
        private readonly ApplicationDbContext _context;


        public Estados(IConfiguration configuration, ApplicationDbContext context)
        {
            this.connectionstring = configuration.GetConnectionString("SQLConnection");
            _context = context;
        }

        [HttpPost]
        [Route("EstadoBascula")]
        public IActionResult EstadoBascula([FromBody] ModeloBascula bascula)
        {
            try
            {
                using (SqlConnection cmd = new SqlConnection(connectionstring))
                {
                    if(cmd.State == ConnectionState.Closed)
                    {
                        cmd.Open();
                    }
                    using(SqlCommand command = new SqlCommand())
                    {
                        command.Connection = cmd;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "Sppdx_AgregarPeso";
                        command.Parameters.AddWithValue("@peso", int.Parse(bascula.Peso));
                        command.Parameters.AddWithValue("@idbascula", int.Parse(bascula.Idbascula));
                        command.ExecuteNonQuery();

                        if(cmd.State == ConnectionState.Open)
                        {
                            cmd.Close();
                        }

                        return Ok();
                    }
                    
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("EstadoSemaforo")]
        public IActionResult EstadosSemaforo([FromQuery] string idbascula)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionstring))
                {
                    if(conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    using(SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "Sppdx_ObtenerEstatus";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idbascula", int.Parse(idbascula));
                        command.Connection = conn;
                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable Table = new DataTable();
                                Table.Load(reader);
                                using (Table)
                                {
                                    conn.Close();

                                    return StatusCode(StatusCodes.Status200OK, new { Estado= Table.Rows[0][0].ToString() });
                                }
                            }
                            else
                            {
                                conn.Close();
                                return BadRequest();
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet]
        [Route("ObtenerPesoBascula")]
        public IActionResult ObtenerPesoBascula([FromHeader] string bascula)
        {
            try
            {
                int idbascula = int.Parse(bascula);

                List<EstadosPesos> pesosBascula = _context.EstadosPesos.Where(x => x.Id_Bascula == idbascula).ToList();

                if (pesosBascula.Count == 0)
                    return StatusCode(StatusCodes.Status404NotFound, new { response = "No existe peso de bascula" });

                return StatusCode(StatusCodes.Status200OK, new { response = pesosBascula[0].PesoBascula });

            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("AddBase64")]
        public IActionResult AddBase64([FromBody] BaseImpresionModel bases)
        {
            try
            {
                string rutaPesos = Directory.GetCurrentDirectory() + "/TicketPTM/base64.txt";

                if (!System.IO.File.Exists(rutaPesos))
                {
                    System.IO.File.Create(rutaPesos).Close();
                }
                else
                {
                    System.IO.File.Delete(rutaPesos);
                    System.IO.File.Create(rutaPesos).Close();
                }

                using (StreamWriter writer = new StreamWriter(rutaPesos))
                {
                    writer.WriteLine(bases.base64);
                    writer.Close();
                }

                return StatusCode(StatusCodes.Status200OK, new { response = "Base 64 impresa correctamente" });
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBaseTicket")]
        public IActionResult GetBaseTicket([FromQuery] int idbascula)
        {
            try
            {
                string rutaPesos = Directory.GetCurrentDirectory() + "/TicketPTM/base64.txt";
                if (!System.IO.File.Exists(rutaPesos))
                {
                    return StatusCode(StatusCodes.Status200OK, new { base64 = "" });
                }
                else
                {
                    var semaforo = _context.EstadosSemaforos.Find(1);
                    if (semaforo != null)
                    {
                        semaforo.Estado = 0;

                        _context.Update(semaforo);
                        _context.SaveChanges();
                    }
                    StreamReader sr = new StreamReader(rutaPesos);
                    string base64 = sr.ReadLine();
                    sr.Close();
                    System.IO.File.Delete(rutaPesos);
                    return StatusCode(StatusCodes.Status200OK, new { base64 });
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
