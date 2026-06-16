using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sap.Data.Hana;
using WebApiPtmHana.BLL.Services.BitacoraHpServ;
using WebApiPtmHana.BLL.Services.HistorialPesadasServ;
using WebApiPtmHana.BLL.Services.LineasServ;
using WebApiPtmHana.Datos;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.HistorialPesadasModel;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialPesadasController : ControllerBase
    {
        private readonly IHistorialPesadas _pesadasService;
        private readonly IBitacoraHPesadas _bitPesadas;
        private readonly IConfiguration _configuration;
        private readonly ILinia _lineas;
        private readonly string connectionString;
        private readonly string prefijo;

        public HistorialPesadasController(IHistorialPesadas pesadasService, IConfiguration configuration, IBitacoraHPesadas bitacoraHPesadas,
            ILinia lineas)
        {
            _pesadasService = pesadasService;
            _bitPesadas = bitacoraHPesadas;
            connectionString = configuration.GetConnectionString("DefaulConnection");
            _lineas = lineas;
            //Llamamos al prefijo para los stores
            prefijo = configuration["StoredProcedurePrefix"];
        }

        [HttpGet]
        [Route("GetAllHistorial")]
        public IActionResult GetAllHistorial([FromHeader] string planta)
        {
            try
            {
                List<InfoOwor> JsonOwor = new List<InfoOwor>();
                HanaResponses hana = new HanaResponses();
                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetSkuPT";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
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
                                    JsonOwor = JsonConvert.DeserializeObject<List<InfoOwor>>(JSONString);
                                }
                            }
                        }
                    }
                }

                int plantaid = 0;
                if(planta == "335")
                {
                    plantaid = 2;
                } else if(planta == "33")
                {
                    plantaid = 1;
                } else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = $"No exíste el identificador {planta} de planta." });
                }


                var historailPesadas = _pesadasService.GetAll(plantaid);
                var lineas = _lineas.CountLineas(plantaid.ToString());
                if (historailPesadas.Count >= 0)
                    return StatusCode(StatusCodes.Status200OK, new { historailPesadas, lineas, JsonOwor });

                return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Error generados dentro del servidor" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("DisableRegister")]
        public async Task<IActionResult> DisabledRecord([FromBody] DesabilitarPtModel model)
        {
            try
            {
                int idHPesada = int.Parse(model.idHistPesada);
                bool result = await _pesadasService.DisabledRegister(idHPesada, model.comentario);
                if (!result)
                    return StatusCode(StatusCodes.Status200OK, new { response = "Error al cambiar el estatus del registro" });

                return StatusCode(StatusCodes.Status200OK, new { response = "Se ha desabilitado correctamente la línea" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetHistoryRecordById")]
        public IActionResult GetHistoryRecordById([FromHeader] string idHistPesada)
        {
            try
            {
                int idHPesada = int.Parse(idHistPesada);
                var productoTerminado = _pesadasService.GetHistoryWeigthById(idHPesada);
                if (productoTerminado.Count == 0)
                    return StatusCode(StatusCodes.Status400BadRequest, new { response = $"No se encuentra registro con el id {idHPesada}" });

                return StatusCode(StatusCodes.Status200OK, new { productoTerminado });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("EditRegisterById")]
        public async Task<IActionResult> EditRecordById([FromBody] EditarHitorialPesadas editPesada)
        {
            try
            {
                bool response = await _pesadasService.EditRecordById(editPesada.idHistPesada, editPesada.LineaPtH, editPesada.PesoTotalPtH, editPesada.NumTubosPtH, editPesada.AtadoPTH,
                    editPesada.TaraPTH, editPesada.AnilloPTH, editPesada.FlejePTH, editPesada.comentario, editPesada.nombreTrabjador, editPesada.idSKU);


                if (!response)
                    return StatusCode(StatusCodes.Status400BadRequest, new { response = "Error dentro del proceso de edición del registro" });

                return StatusCode(StatusCodes.Status200OK, new { response = "Edición generada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("GetBitacoraPesadas")]
        public IActionResult GetBitacoraPesadas([FromHeader] string idHPesadas)
        {
            try
            {
                int idPesdas = int.Parse(idHPesadas);
                var bitacoraHPesadas = _bitPesadas.GetHistoricalProducts(idPesdas);

                return StatusCode(StatusCodes.Status200OK, new { bitacoraHPesadas });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("CopyRegister")]
        public IActionResult CopyRecord([FromBody] CopiaRegistroModel modelRecord)
        {
            try
            {
                int idPesadas = int.Parse(modelRecord.idHPesado);
                string comentario = modelRecord.comentario;
                string Codigo = modelRecord.Codigo;
                int Id_Linea = modelRecord.Id_Linea;
                float PesoTotal = modelRecord.PesoTotal;
                int NumTubos = modelRecord.NumTubos;
                float AtadoPT = modelRecord.AtadoPT;
                float TaraPT = modelRecord.TaraPT;
                float AnilloPT = modelRecord.AnilloPT;
                float FlejePT = modelRecord.FlejePT;

                bool result = _pesadasService.CopyRecord(idPesadas, comentario, Codigo, Id_Linea, PesoTotal
                    , NumTubos, AtadoPT, TaraPT, AnilloPT, FlejePT);
                if (!result)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = "Error al copiar el registro" });

                return StatusCode(StatusCodes.Status200OK, new { response = "Registro copiado de manera correcta" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        [HttpGet]
        [Route("FilterHistorical")]
        public IActionResult FilterHistorical([FromHeader] string? turno, [FromHeader] string? codigo, [FromHeader] string? idEstadoSap,
            [FromHeader] string? fechaInicio, [FromHeader] string? fechaFin, [FromHeader] string planta, [FromHeader] string corte,
             [FromHeader] string? linea)
        {
            try
            {

                List<InfoOwor> JsonOwor = new List<InfoOwor>();
                HanaResponses hana = new HanaResponses();

                int plantaid = 0;
                if (planta == "335")
                {
                    plantaid = 2;
                }
                else if (planta == "33")
                {
                    plantaid = 1;
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { response = $"No exíste el identificador {planta} de planta." });
                }

                using (HanaConnection connection = new HanaConnection(connectionString))
                {
                    connection.Open();
                    using (HanaCommand command = new HanaCommand())
                    {
                        command.CommandText = $"{prefijo}.SPPDX_GetSkuPT";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
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
                                    JsonOwor = JsonConvert.DeserializeObject<List<InfoOwor>>(JSONString);
                                }
                            }
                        }
                    }
                }
                if (turno == null && (fechaInicio != null && fechaFin != null))
                {
                    fechaInicio = $"{fechaInicio} 04:30:00";
                    fechaFin = $"{fechaFin} 04:30:00";
                }
                               
                    if (corte != null && fechaInicio != null)
                    {
                        switch (corte.ToString())
                        {
                        case "1":
                            fechaInicio = $"{fechaInicio} 04:30:00";
                            fechaFin = $"{fechaInicio.Replace("04:30:00", "")} 11:00:00";
                            break;
                        case "2":
                            fechaInicio = $"{fechaInicio} 11:00:00";
                            fechaFin = $"{fechaInicio.Replace("11:00:00", "")} 16:30:00";
                            break;
                        case "3":
                            fechaInicio = $"{fechaInicio} 04:30:00";
                            fechaFin = $"{fechaInicio.Replace("04:30:00", "")} 16:30:00";
                            break;
                        case "4":
                            fechaInicio = $"{fechaInicio} 16:30:00";
                            fechaFin = $"{fechaInicio.Replace("16:30:00", "")} 23:00:00";
                            break;
                        case "5":
                            fechaInicio = $"{fechaInicio} 23:00:00";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                            break;
                        case "6":
                            fechaInicio = $"{fechaInicio} 16:30:00";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                            break;
                        case "7":
                            fechaInicio = $"{fechaInicio} 04:30:00";
                            fechaFin = $"{DateTime.Parse(fechaInicio).AddDays(1).ToString("yyyy/MM/dd")} 04:30:00";
                            break;
                    };
                    }
                
                //Devuelve las lineas de produccion eistentes
                var lineas = _lineas.CountLineas(plantaid.ToString()); 
                //Filtrado de los escaneos
                var filtradoHP = _pesadasService.FilterInfoHistorical(turno == null ? null : int.Parse(turno),
                    codigo == null ? null : codigo, idEstadoSap == null ? null : int.Parse(idEstadoSap),
                    fechaInicio == null ? null : fechaInicio, fechaFin == null ? null : fechaFin,
                    plantaid, linea == null ? null : int.Parse(linea));

                return StatusCode(StatusCodes.Status200OK, new { filtradoHP, JsonOwor, lineas });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { ex.Message });
            }
        }
    }
}
