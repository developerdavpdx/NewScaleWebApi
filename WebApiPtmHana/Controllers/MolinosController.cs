using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using WebApiPtmHana.Models.DatosMolinos;


namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MolinosController : Controller
    {
        private readonly string connectionString = "";

        public MolinosController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("SQLConnection");
        }

        //--------------------------CONTROLLERS

        /// <summary>
        /// Agregamos o eliminamos un molino
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AgregarQuitarMolino")]
        public IActionResult AgregarQuitarMolino([FromHeader] string NumeroMolino, [FromHeader] string Proceso,
            [FromHeader] string HorasxTurno, [FromHeader] string Eficiencia, [FromHeader] string Borrar,
            [FromHeader] string Planta, [FromHeader] string ObjetivoProduccion, [FromHeader] string Nombre)
        {
            try
            {

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_AgregarQuitarMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NumeroMolino", int.Parse(NumeroMolino));
                        command.Parameters.AddWithValue("@Proceso", (Proceso));
                        command.Parameters.AddWithValue("@HorasxTurno", (HorasxTurno));
                        command.Parameters.AddWithValue("@Eficiencia", (Eficiencia));
                        command.Parameters.AddWithValue("@Borrar", int.Parse(Borrar));
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@ObjetivoProduccion", (ObjetivoProduccion));
                        command.Parameters.AddWithValue("@Nombre", (Nombre));
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Molino Agregado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos los molinos existentes
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerNumeroMolinos")]
        public IActionResult ObtenerNumeroMolinos([FromHeader] string Planta)
        {
            try
            {

                string JSONstringSp = string.Empty;
                List<MolinosLineas> DatosMolinos = new List<MolinosLineas>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_ObtenerMolinosData";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosMolinos = JsonConvert.DeserializeObject<List<MolinosLineas>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosMolinos });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Actualizamos un molino
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ActualizarMolino")]
        public IActionResult ActualizarMolino([FromHeader] string NumeroMolino, [FromHeader] string Eficiencia,
            [FromHeader] string HorasxTurno, [FromHeader] string Planta, [FromHeader] string ObjetivoProduccion)
        {
            try
            {

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_ActualizarMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NumeroMolino", int.Parse(NumeroMolino));
                        command.Parameters.AddWithValue("@HorasxTurno", (HorasxTurno));
                        command.Parameters.AddWithValue("@Eficiencia", (Eficiencia));
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@ObjetivoProduccion", (ObjetivoProduccion));
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Molino Actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los molinos en paro
        /// </summary>
        /// <param name="NumeroMolinoParo"></param>
        /// <param name="PlantaParo"></param>
        /// <param name="FechaYHoraParo"></param>
        /// <param name="Paro"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ActualizarParoMolino")]
        public IActionResult ActualizarParoMolino([FromHeader] string NumeroMolinoParo, [FromHeader] string PlantaParo,
            [FromHeader] string FechaYHoraParo, [FromHeader] string Paro)
        {
            try
            {

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_ActualizarParoMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NumeroMolino", int.Parse(NumeroMolinoParo));
                        command.Parameters.AddWithValue("@FechaYHora", (FechaYHoraParo));
                        command.Parameters.AddWithValue("@Planta", int.Parse(PlantaParo));
                        command.Parameters.AddWithValue("@Paro", (int.Parse(Paro)));
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Molino Actualizado en paro" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos los registros (historial y concentrado) de peso de molinos Filtrados por fecha y turno
        /// </summary>
        /// <param name="Planta"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="procFechaFineso"></param>
        /// <param name="Turno"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerDatosReporteMolinos")]
        public IActionResult ObtenerDatosReporteMolinos([FromHeader] string Planta, [FromHeader] string FechaInicio,
            [FromHeader] string FechaFin, [FromHeader] string Turno, [FromHeader] string Histori)
        {
            try
            {
                //parseamos las fechas para poder manipular las horas
                DateTime fechaInicioControler = DateTime.Parse(FechaInicio);
                DateTime fechaFinControler = DateTime.Parse(FechaFin);
                //obtenemos la hora actual
                DateTime horaActual = DateTime.Now;
                //REVISAMOS QUE TURNO ES PARA ELEGIR EL HORARIO A ASIGNAR
                switch (Turno)
                {
                    case "1": //Turno 1
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(5, 45, 1)); // 05:45:01 AM
                        fechaFinControler = fechaFinControler.Date.Add(new TimeSpan(17, 45, 0)); // 05:45:00 PM
                        break;
                    case "2": //Turno 2
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(17, 45, 1)); // 05:45:01 PM
                        fechaFinControler = fechaFinControler.Date.AddDays(1).Date.Add(new TimeSpan(5, 45, 0)); // 05:45:00 AM del día siguiente

                        //si es el turno 2 y ya es de madrugada elejimos la horainicio de ayer, y hora fin de hoy
                        if (horaActual.Hour >= 0 && horaActual.Hour < 5 || (horaActual.Hour == 5 && horaActual.Minute <= 45))
                        {
                            fechaFinControler = fechaFinControler.AddDays(-1);
                            fechaInicioControler = fechaInicioControler.AddDays(-1);
                        }
                        break;
                    case "3": //Reporte del dia
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(5, 45, 1)); // 05:45:01 AM
                        fechaFinControler = fechaFinControler.Date.AddDays(1).Date.Add(new TimeSpan(5, 45, 0)); // 05:45:00 AM del día siguiente
                        break;
                    case "4": //Si no se selecciono un turno, elejimos el horario de inicio del dia inicio, hasta el horario final del ultimo dia seleccionado (un dia despues del dia fin)
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(5, 45, 1)); // 04:30:01 AM
                        fechaFinControler = fechaFinControler.Date.AddDays(1).Add(new TimeSpan(17, 45, 0)); // 04:30:00 PM
                        break;
                }


                string JSONstringSp = string.Empty;
                List<MolinosRegistrosPesos> DatosMolinosReporte = new List<MolinosRegistrosPesos>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_ObtenerDatosReporteMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@FechaInicio", fechaInicioControler);
                        command.Parameters.AddWithValue("@FechaFin", fechaFinControler);
                        command.Parameters.AddWithValue("@Turno", Turno);
                        command.Parameters.AddWithValue("@Histori", Histori); //indicamos si estamos consultando el historial (1) o el concentrado (0)

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosMolinosReporte = JsonConvert.DeserializeObject<List<MolinosRegistrosPesos>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosMolinosReporte });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos Datos para el Dashboard
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DatosDashboard")]
        public IActionResult DatosDashboard([FromHeader] string Planta, [FromHeader] string Turno,
            [FromHeader] string FechaInicio, [FromHeader] string FechaFin)
        {
            try
            {

                //parseamos las fechas para poder manipular las horas
                DateTime fechaInicioControler = DateTime.Parse(FechaInicio);
                DateTime fechaFinControler = DateTime.Parse(FechaFin);
                //obtenemos la hora actual
                DateTime horaActual = DateTime.Now;
                //REVISAMOS QUE TURNO ES PARA ELEJIR EL HORARIO A ASIGNAR
                switch (Turno)
                {
                    case "1":
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(5, 45, 1)); // 05:45:01 AM
                        fechaFinControler = fechaFinControler.Date.Add(new TimeSpan(17, 45, 0)); // 05:45:00 PM
                        break;
                    case "2":
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(17, 45, 1)); // 05:45:01 PM
                        fechaFinControler = fechaFinControler.Date.AddDays(1).Date.Add(new TimeSpan(5, 45, 0)); // 05:45:00 AM del día siguiente

                        //si es el turno 2 y ya es de madrugada elejimos la horainicio de ayer, y hora fin de hoy
                        if (horaActual.Hour >= 0 && horaActual.Hour < 5 || (horaActual.Hour == 4 && horaActual.Minute <= 45))
                        {
                            fechaFinControler = fechaFinControler.AddDays(-1);
                            fechaInicioControler = fechaInicioControler.AddDays(-1);
                        }
                        break;
                }

                string JSONstringSp = string.Empty;
                List<DatosDashboardMolinos> DashboardData = new List<DatosDashboardMolinos>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_DashboardMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@Turno", Turno);
                        command.Parameters.AddWithValue("@FechaInicio", fechaInicioControler);
                        command.Parameters.AddWithValue("@FechaFin", fechaFinControler);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DashboardData = JsonConvert.DeserializeObject<List<DatosDashboardMolinos>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DashboardData });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos los Articulos (Codigos)
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerArticulosMolinos")]
        public IActionResult ObtenerArticulosMolinos([FromHeader] string Planta)
        {
            try
            {
                string JSONstringSp = string.Empty;
                List<ArticulosCodigosMolinos> ArticulosMolinos = new List<ArticulosCodigosMolinos>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_ObtenerArticulosDeMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                ArticulosMolinos = JsonConvert.DeserializeObject<List<ArticulosCodigosMolinos>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { ArticulosMolinos });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Validamos que el codigo exista
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ValidarCodigo")]
        public IActionResult ValidarCodigo([FromHeader] string Planta, [FromHeader] string Codigo)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_ValidarCodigoMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@Codigo", Codigo);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //guardamos los campos que estamos devolviendo desde el procedure (si se encontro el codigo) para mandarlos al front
                                var result = new
                                {
                                    Codigo = reader["Codigo"],
                                    Proceso = reader["Proceso"],
                                    Planta = reader["Planta"],
                                    Descripcion = reader["Descripcion"],
                                    FechaCreacion = reader["FechaCreacion"],
                                    Tipo = reader["Tipo"],
                                    Familia = reader["Familia"]
                                };

                                return StatusCode(StatusCodes.Status200OK, new { message = "Codigo Encontrado", data = result });
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status404NotFound, new { message = "Codigo Encontrado, pero sin datos" });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 50000) // Maneja el error específico de "Código no encontrado"
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Codigo de scrap invalido, revisar que exista en los datos maestros" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos Las horas programadas y objetivo de produccion del molino donde se hara el registro, tambiejn obtenemos su nombre (Proceso)
        /// </summary>
        /// <param name="Linea"></param>
        /// <param name="Planta"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerHorasyObjetivoMolino")]
        public IActionResult ObtenerHorasyObjetivoMolino([FromHeader] string Linea, [FromHeader] string Planta)
        {
            try
            {
                string JSONstringSp = string.Empty;
                List<DatosparaRegistroMolino> DatosObjMolinos = new List<DatosparaRegistroMolino>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_ObtenerObjetivosparaRegistroMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Linea", Linea);
                        command.Parameters.AddWithValue("@Planta", Planta);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosObjMolinos = JsonConvert.DeserializeObject<List<DatosparaRegistroMolino>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosObjMolinos });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Agregamos un escaneo al registro
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AgregarEscaneo")]
        public IActionResult AgregarEscaneo([FromHeader] string Molino, [FromHeader] string Turno,
            [FromHeader] string FechaRegistro, [FromHeader] string Operador,
            [FromHeader] string ObjetivoProduccion, [FromHeader] string Codigo, [FromHeader] string PesoTotal,
            [FromHeader] string HorasProgramadas, [FromHeader] string HorasParo, [FromHeader] string Planta,
            [FromHeader] string ComentariosParo, [FromHeader] string MotivosParo, [FromHeader] string Folio, [FromHeader] string NombreMolino,
            [FromHeader] string Material)
        {
            try
            {
                //verificamos si viene "N/A en codigosparo o comentario paro para asignar ""
                if (ComentariosParo == "N/A")
                {
                    ComentariosParo = "";
                }
                if (MotivosParo == "N/A")
                {
                    MotivosParo = "";
                }

                //algunos parametros vienen codificados porque tienen caracteres especiales, aqui los decodificamos
                if (!string.IsNullOrWhiteSpace(MotivosParo))
                {
                    MotivosParo = Uri.UnescapeDataString(MotivosParo);
                }
                if (!string.IsNullOrWhiteSpace(ComentariosParo))
                {
                    ComentariosParo = Uri.UnescapeDataString(ComentariosParo);
                }
                if (!string.IsNullOrWhiteSpace(Operador))
                {
                    Operador = Uri.UnescapeDataString(Operador);
                }
                if (!string.IsNullOrWhiteSpace(Material))
                {
                    Material = Uri.UnescapeDataString(Material);
                }

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_AgregarEscaneoMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Molino", int.Parse(Molino));
                        command.Parameters.AddWithValue("@Turno", int.Parse(Turno));
                        command.Parameters.AddWithValue("@FechaRegistro", (FechaRegistro));
                        command.Parameters.AddWithValue("@Operador", (Operador));
                        command.Parameters.AddWithValue("@ObjetivoProduccion", (ObjetivoProduccion));
                        command.Parameters.AddWithValue("@Codigo", (Codigo));
                        command.Parameters.AddWithValue("@PesoTotal", (PesoTotal));
                        command.Parameters.AddWithValue("@HorasProgramadas", (HorasProgramadas));
                        command.Parameters.AddWithValue("@HorasParo", (HorasParo));
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@ComentariosParo", ComentariosParo);
                        command.Parameters.AddWithValue("@MotivosParo", MotivosParo);
                        command.Parameters.AddWithValue("@Folio", Folio);
                        command.Parameters.AddWithValue("@NombreMolino", NombreMolino);
                        command.Parameters.AddWithValue("@Material", Material);
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Molino Agregado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Agrega, elimina o actualiza un codifo maestro, dependiendo de la variable borrar [0 agrega, 1 elmina, 2 actualiza]
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Proceso"></param>
        /// <param name="Planta"></param>
        /// <param name="Descripcion"></param>
        /// <param name="FechaCreacion"></param>
        /// <param name="Tipo"></param>
        /// <param name="Familia"></param>
        /// <param name="Borrar"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AgregarEliminarActualizarCodeArticulos")]
        public IActionResult AgregarEliminarActualizarCodeArticulos([FromHeader] string Codigo, [FromHeader] string Proceso,
            [FromHeader] string Planta, [FromHeader] string Descripcion, [FromHeader] string FechaCreacion, [FromHeader] string Tipo, [FromHeader] string Familia, [FromHeader] string Borrar)
        {
            try
            {

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_AgregarEliminarUpdateCodigoDatoMaestroMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Codigo", Codigo);
                        command.Parameters.AddWithValue("@Proceso", (Proceso));
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@Descripcion", (Descripcion));
                        command.Parameters.AddWithValue("@FechaCreacion", (FechaCreacion));
                        command.Parameters.AddWithValue("@Tipo", (Tipo));
                        command.Parameters.AddWithValue("@Familia", (Familia));
                        command.Parameters.AddWithValue("@Borrar", (Borrar));
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Codigo agregado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Actualizamos un registro en sus horas paro
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="turno"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ActualizaHorasParoRegistro")]
        public IActionResult ActualizaHorasParoRegistro([FromHeader] string Planta, [FromHeader] string Comentarios, [FromHeader] string HorasParo, [FromHeader] string Folio,
        [FromHeader] string Peso, [FromHeader] string Desactivar)
        {
            try
            {
                //algunos parametros vienen codificados porque tienen caracteres especiales, aqui los decodificamos
                if (!string.IsNullOrWhiteSpace(Comentarios))
                {
                    Comentarios = Uri.UnescapeDataString(Comentarios);
                }
                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_ActualizarHorasParoRegistroMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Folio", int.Parse(Folio));
                        command.Parameters.AddWithValue("@HorasParo", (HorasParo));
                        command.Parameters.AddWithValue("@Comentarios", (Comentarios));
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@Peso", Peso);
                        command.Parameters.AddWithValue("@Desactivar", int.Parse(Desactivar));
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Registro Actualizado" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Validamos que el molinero exista y devolvemos su nombre
        /// </summary>
        /// <param name="planta"></param>
        /// <param name="CodigoMolinero"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMolinero")]
        public IActionResult ObtenerMolinero([FromHeader] string CodigoMolinero, [FromHeader] string Planta)
        {
            try
            {
                string Molinero = null;
                string Activo = null; // Usa un tipo nullable por si no hay resultados

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_BuscarMolinerouserMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@CodigoMolinero", CodigoMolinero);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Si hay resultados
                            {
                                Molinero = reader["Molinero"] as string;
                                Activo = reader["Activo"] as string; // Convierte a bool nullable
                            }
                        }
                    }
                }

                if (Molinero != null)
                {
                    return StatusCode(StatusCodes.Status200OK, new { message = "Codigo Encontrado", Molinero, Activo });
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "Codigo no Encontrado" });
                }
            }
            catch (SqlException ex) when (ex.Number == 50000) // Maneja el error específico de "Código no encontrado"
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = "Codigo invalido, revisar que el molinero exista en el registro" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos los molineros existentes
        /// </summary>
        /// <param name="planta"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerMolinerosMolinos")]
        public IActionResult ObtenerMolinerosMolinos([FromHeader] string Planta)
        {
            try
            {
                string JSONstringSp = string.Empty;
                List<MolinerosMolinos> MolinerosMolinos = new List<MolinerosMolinos>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_ObtenerMolinerosDeMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                MolinerosMolinos = JsonConvert.DeserializeObject<List<MolinerosMolinos>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { MolinerosMolinos });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Agregar o eliminar un nuevo operador (Molinero)
        /// </summary>
        /// <param name="Planta"></param>
        /// <param name="Codigo"></param>
        /// <param name="Nombre"></param>
        /// <param name="Borrar"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AgregarEliminarOperador")]
        public IActionResult AgregarEliminarOperador([FromHeader] string Codigo, [FromHeader] string Nombre,
            [FromHeader] string Borrar, [FromHeader] string Planta, [FromHeader] string Fecha, [FromHeader] string Activo, [FromHeader] string CodigoUserEdit)
        {
            try
            {
                //algunos parametros vienen codificados porque tienen caracteres especiales, aqui los decodificamos
                if (!string.IsNullOrWhiteSpace(Nombre))
                {
                    Nombre = Uri.UnescapeDataString(Nombre);
                }

                string JSONstringSp = string.Empty;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "Sppdx_AgregarEliminarOperadorMolino";
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Codigo", Codigo);
                        command.Parameters.AddWithValue("@Nombre", Nombre);
                        command.Parameters.AddWithValue("@Borrar", int.Parse(Borrar));
                        command.Parameters.AddWithValue("@Planta", int.Parse(Planta));
                        command.Parameters.AddWithValue("@Fecha", Fecha);
                        command.Parameters.AddWithValue("@Activo", Activo);
                        command.Parameters.AddWithValue("@CodigoUserEdit", CodigoUserEdit);
                        command.ExecuteNonQuery();

                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { message = "Operador modificado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// Obtenemos los registros de ingresos de scrap
        /// </summary>
        /// <param name="Planta"></param>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="Turno"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ObtenerDatosScrapIngresos")]
        public IActionResult ObtenerDatosScrapIngresos([FromHeader] string Planta, [FromHeader] string FechaInicio,
            [FromHeader] string FechaFin, [FromHeader] string Turno) //Nota, este procedure no usa "turno", solo se usa para elegir los horarios de las fechas
        {
            try
            {
                //parseamos las fechas para poder manipular las horas
                DateTime fechaInicioControler = DateTime.Parse(FechaInicio);
                DateTime fechaFinControler = DateTime.Parse(FechaFin);
                //obtenemos la hora actual
                DateTime horaActual = DateTime.Now;
                //REVISAMOS QUE TURNO ES PARA ELEGIR EL HORARIO A ASIGNAR
                switch (Turno)
                {
                    case "1":
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(4, 30, 1)); // 04:30:01 AM
                        fechaFinControler = fechaFinControler.Date.Add(new TimeSpan(16, 30, 0)); // 04:30:00 PM
                        break;
                    case "2":
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(16, 30, 1)); // 04:30:01 PM
                        fechaFinControler = fechaFinControler.Date.AddDays(1).Date.Add(new TimeSpan(4, 30, 0)); // 04:30:00 AM del día siguiente

                        //si es el turno 2 y ya es de madrugada elejimos la horainicio de ayer, y hora fin de hoy
                        if (horaActual.Hour >= 0 && horaActual.Hour < 4 || (horaActual.Hour == 4 && horaActual.Minute <= 30))
                        {
                            fechaFinControler = fechaFinControler.AddDays(-1);
                            fechaInicioControler = fechaInicioControler.AddDays(-1);
                        }
                        break;
                    case "3": //"sin turno", seleccionamos ambos turnos en el procedure (de 4:30am del dia inicial a 4:30 am del siguiente dia)
                        fechaInicioControler = fechaInicioControler.Date.Add(new TimeSpan(4, 30, 1)); // 04:30:01 AM
                        fechaFinControler = fechaFinControler.Date.AddDays(1).Add(new TimeSpan(4, 30, 0)); // 04:30:00 am del siguiente dia
                        break;
                }


                string JSONstringSp = string.Empty;
                List<MolinosRegistrosIngresos> DatosMolinosIngresos = new List<MolinosRegistrosIngresos>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_ObtenerDatosIngresosMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@FechaInicio", fechaInicioControler);
                        command.Parameters.AddWithValue("@FechaFin", fechaFinControler);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosMolinosIngresos = JsonConvert.DeserializeObject<List<MolinosRegistrosIngresos>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosMolinosIngresos });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }

        /// <summary>
        /// CRUD para Los Motivos de Paro
        /// </summary>
        /// <param name="Planta"></param>
        /// <param name="Accion"></param>
        /// <param name="Motivo"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CRUDMotivosDeParoMolinos")]
        public IActionResult CRUDMotivosDeParoMolinos([FromHeader] string Planta, [FromHeader] string Accion, [FromHeader] string Motivo, [FromHeader] string Id)
        {
            try
            {
                //El motivo puede venir codificado porque puede tener caracteres especiales, aqui lo decodificamos
                if (!string.IsNullOrWhiteSpace(Motivo))
                {
                    Motivo = Uri.UnescapeDataString(Motivo);
                }

                string JSONstringSp = string.Empty;
                List<MotivosDeParo> DatosMotivosParo = new List<MotivosDeParo>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_CRUDMotivosDeParoMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@Accion", Accion);
                        command.Parameters.AddWithValue("@Motivo", Motivo);
                        command.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosMotivosParo = JsonConvert.DeserializeObject<List<MotivosDeParo>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosMotivosParo });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        /// <summary>
        /// CRUD para las familias de los articulos
        /// </summary>
        /// <param name="Planta"></param>
        /// <param name="Accion"></param>
        /// <param name="Familia"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// Aqui es nuevo <---
        [HttpGet]
        [Route("CRUDFamiliasMolinos")]
        public IActionResult CRUDFamiliasMolinos([FromHeader] string Planta, [FromHeader] string Accion, [FromHeader] string Familia, [FromHeader] string Id)
        {
            try
            {
                //La familia puede venir codificado porque puede tener caracteres especiales, aqui lo decodificamos
                if (!string.IsNullOrWhiteSpace(Familia))
                {
                    Familia = Uri.UnescapeDataString(Familia);
                }

                string JSONstringSp = string.Empty;
                List<Familias> DatosFamilias = new List<Familias>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_CRUDFamiliasMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@Accion", Accion);
                        command.Parameters.AddWithValue("@Familia", Familia);
                        command.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosFamilias = JsonConvert.DeserializeObject<List<Familias>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosFamilias });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        /// <summary>
        /// CRUD para los tipos de material de losarticulos
        /// </summary>
        /// <param name="Planta"></param>
        /// <param name="Accion"></param>
        /// <param name="Familia"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// Aqui es nuevo <---
        [HttpGet]
        [Route("CRUDTiposMaterialMolinos")]
        public IActionResult CRUDTiposMaterialMolinos([FromHeader] string Planta, [FromHeader] string Accion, [FromHeader] string Tipo, [FromHeader] string Proceso, [FromHeader] string Id)
        {
            try
            {
                //El tipo puede venir codificado porque puede tener caracteres especiales, aqui lo decodificamos
                if (!string.IsNullOrWhiteSpace(Tipo))
                {
                    Tipo = Uri.UnescapeDataString(Tipo);
                }

                //El Proceso puede venir codificado porque puede tener caracteres especiales, aqui lo decodificamos
                if (!string.IsNullOrWhiteSpace(Proceso))
                {
                    Proceso = Uri.UnescapeDataString(Proceso);
                }

                string JSONstringSp = string.Empty;
                List<TiposDeMaterial> DatosTipoMaterial = new List<TiposDeMaterial>();


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_CRUDTiposMaterialMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@Accion", Accion);
                        command.Parameters.AddWithValue("@Tipo", Tipo);
                        command.Parameters.AddWithValue("@Proceso", Proceso);
                        command.Parameters.AddWithValue("@Id", Id);

                        using (SqlDataAdapter daCommand = new SqlDataAdapter(command))
                        using (DataSet dtDocuments = new DataSet())
                        {
                            daCommand.Fill(dtDocuments);
                            DataTable tabla1 = dtDocuments.Tables[0];

                            using (tabla1)
                            {
                                JSONstringSp = JsonConvert.SerializeObject(tabla1);
                                DatosTipoMaterial = JsonConvert.DeserializeObject<List<TiposDeMaterial>>(JSONstringSp);
                            }
                        }

                        return StatusCode(StatusCodes.Status200OK, new { DatosTipoMaterial });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        /// <summary>
        /// CRUD para los ingresos
        /// </summary>
        /// <param name="Accion"></param>
        /// <param name="Id"></param>
        /// <param name="Planta"></param>
        /// <param name="Codigo"></param>
        /// <param name="Tipo"></param>
        /// <param name="Familia"></param>
        /// <param name="Fecha"></param>
        /// <param name="Peso"></param>
        /// <param name="Origen"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CRUDRegistroIngresos")]
        public IActionResult CRUDRegistroIngresos([FromHeader] string Accion, [FromHeader] string Id, [FromHeader] string Planta, [FromHeader] string Codigo,
        [FromHeader] string Tipo, [FromHeader] string Familia, [FromHeader] string Fecha, [FromHeader] string Peso, [FromHeader] string Origen)
        {
            try
            {
                // Decodifica los parámetros si vienen codificados
                Codigo = !string.IsNullOrWhiteSpace(Codigo) ? Uri.UnescapeDataString(Codigo) : Codigo;
                Tipo = !string.IsNullOrWhiteSpace(Tipo) ? Uri.UnescapeDataString(Tipo) : Tipo;
                Familia = !string.IsNullOrWhiteSpace(Familia) ? Uri.UnescapeDataString(Familia) : Familia;
                Origen = !string.IsNullOrWhiteSpace(Origen) ? Uri.UnescapeDataString(Origen) : Origen;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "Sppdx_CRUDRegistroIngresosMolinos";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Accion", Accion);
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Planta", Planta);
                        command.Parameters.AddWithValue("@Codigo", Codigo);
                        command.Parameters.AddWithValue("@Tipo", Tipo);
                        command.Parameters.AddWithValue("@Familia", Familia);
                        command.Parameters.AddWithValue("@Fecha", Fecha);
                        command.Parameters.AddWithValue("@Peso", Peso);
                        command.Parameters.AddWithValue("@Origen", Origen);

                        command.ExecuteNonQuery(); // Ejecuta el procedimiento almacenado

                        return StatusCode(StatusCodes.Status200OK, new { message = "Procedimiento ejecutado correctamente." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Guarda los datos extraidos del excel (Ingresos Scrap) en la BD
        /// </summary>
        /// <param name="datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GuardarDatosExcelIngresosEnBD")]
        public IActionResult GuardarDatosExcelIngresosEnBD([FromBody] List<Dictionary<string, string>> datos)
        {
            try
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("Codigo", typeof(string));
                dataTable.Columns.Add("Tipo", typeof(string));
                dataTable.Columns.Add("Familia", typeof(string));
                dataTable.Columns.Add("Peso", typeof(decimal));
                dataTable.Columns.Add("FechaRegistro", typeof(DateTime));
                dataTable.Columns.Add("Origen", typeof(string));
                dataTable.Columns.Add("Planta", typeof(string));

                foreach (var item in datos)
                {
                    dataTable.Rows.Add(
                        item["Codigo"],
                        item["Tipo"],
                        item["Familia"],
                        decimal.Parse(item["Peso"]),
                        DateTime.Parse(item["FechaRegistro"]),
                        item["Origen"],
                        item["Planta"]
                    );
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("Sppdx_GuardarDatosMolinoIngresos", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        var parameter = command.Parameters.AddWithValue("@Ingresos", dataTable);
                        parameter.SqlDbType = SqlDbType.Structured;
                        command.ExecuteNonQuery();
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Datos guardados correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        //nextcontroller
    }
}
