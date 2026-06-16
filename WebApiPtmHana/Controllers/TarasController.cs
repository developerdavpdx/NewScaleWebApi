using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebApiPtmHana.BLL.Services.TarasServ;
using WebApiPtmHana.Models.TareUpdate;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarasController : ControllerBase
    {
        private readonly ITaras _taras;
        private readonly string connectionStringSQL;

        public TarasController(ITaras taras, IConfiguration configuration)
        {
            _taras = taras;
            connectionStringSQL = configuration.GetConnectionString("SQLConnection");
        }

        /// <summary>
        /// Actualizacion masiva de taras
        /// </summary>
        /// <param name="taras"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateTare")]
        public async Task<IActionResult> UpdateTare([FromBody] ExcelTareUpdate taras)
        {
            try
            {   
                //actualizamos las taras en la tabla de SKUinfo
                bool response = await _taras.UpdateTaras(taras.tarasInfo);

                if (!response)
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Error al actualizar las taras de los códigos" });

                return StatusCode(StatusCodes.Status200OK, new { message = "Se han actualizado los códigos de manera correcta" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizacion masiva de KgHora
        /// </summary>
        /// <param name="kgHora"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateKgHora")]
        public async Task<IActionResult> UpdateTUpdateKgHora([FromBody] ExcelKgHoraUpdate kgHoraData)
        {
            try
            {
                // Deserializamos KgHoraInfo que viene como un string en JSON
                var listaKgHora = JsonConvert.DeserializeObject<List<string[]>>(kgHoraData.KgHoraInfo);

                using (SqlConnection cnn = new SqlConnection(connectionStringSQL))
                {
                    if (cnn.State == ConnectionState.Closed)
                    {
                        cnn.Open();
                    }
                    foreach (var registro in listaKgHora)
                    {
                        // Asumimos que los elementos están en el orden correcto
                        string codigo = registro[0];
                        //decimal kgHora = Convert.ToDecimal(registro[1].Replace(",", ".")); // Convertimos a decimal
                        decimal kgHora = Convert.ToDecimal(registro[1], CultureInfo.InvariantCulture); // usamos cultureinfo, ya que la configuracion regional del servidor interfiere con la conversion y elimina el punto decimal
                        int linea = Convert.ToInt32(registro[2]);
                        int planta = Convert.ToInt32(registro[3]);

                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = cnn;
                            command.CommandText = "Sppdx_ActualizarMasiveKgHoraPorLinea";
                            command.CommandType = CommandType.StoredProcedure;

                            // Parámetros del procedure
                            command.Parameters.AddWithValue("@Codigo", codigo);
                            command.Parameters.AddWithValue("@KgHora", kgHora);
                            command.Parameters.AddWithValue("@Linea", linea);
                            command.Parameters.AddWithValue("@Planta", planta);

                            command.ExecuteNonQuery();
                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Actualización exitosa." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }

        }
    }
}
