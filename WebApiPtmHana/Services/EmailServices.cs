using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using WebApiPtmHana.Models;
using WebApiPtmHana.Models.Reportes;

namespace WebApiPTMTest.Services
{
	public class EmailServices
	{
		public List<ReportesProdTerm> ObtenerDatosReporte(int turno,string proceso,DateTime fechaTurnoInicio,DateTime fechaTurnoFin)
		{
			try
			{

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                string connectionstring = configuration.GetConnectionString("SQLConnection");
                // Crear la conexión a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    // Abrir la conexión
                    connection.Open();

                    // Crear el comando para ejecutar el procedimiento almacenado
                    using (SqlCommand command = new SqlCommand("DatosReporte", connection))
                    {
                        // Especificar que el comando es un procedimiento almacenado
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros necesarios
                        command.Parameters.AddWithValue("@turno", turno); // Reemplaza 1 con el valor deseado
                        command.Parameters.AddWithValue("@proceso", proceso); // Reemplaza "proceso" con el valor deseado
                        command.Parameters.AddWithValue("@FechaTurnoInicio", fechaTurnoInicio); // Reemplaza DateTime.Now con el valor deseado
                        command.Parameters.AddWithValue("@FechaTurnoFin", fechaTurnoFin); // Reemplaza DateTime.Now con el valor deseado

                        // Crear un SqlDataReader para leer los resultados del procedimiento almacenado
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Lista para almacenar los objetos reporte
                            List<ReportesProdTerm> reportes = new List<ReportesProdTerm>();

                            // Leer cada fila de resultados y deserializar en objetos reporte
                            while (reader.Read())
                            {
                                ReportesProdTerm r = new ReportesProdTerm
                                {
                                    Id_Linea =  Convert.ToInt32(reader["Id_Linea"].ToString()),
                                    Codigo = reader["Codigo"].ToString(),
                                    AtadosTarimas = Convert.ToInt32(reader["AtadosTarimas"].ToString()),
                                    NumTubos = Convert.ToInt32(reader["NumTubos"].ToString()),
                                    PesoTotal = Convert.ToDecimal(reader["PesoTotal"].ToString()),
                                    PUnitEstandar = Convert.ToDecimal(reader["PUnitEstandar"].ToString()),
                                    PUnitReal = Convert.ToDecimal(reader["PUnitReal"].ToString()),
                                    SobrePeso = Convert.ToDecimal(reader["SobrePeso"].ToString()),
                                    Scrap = Convert.ToDecimal(reader["Scrap"].ToString()),
                                    Eficiencia = Convert.ToDecimal(reader["Eficiencia"].ToString())
                                };

                                reportes.Add(r);
                            }

                            return reportes;
                        }
                    }
                    // Cerrar la conexión
                    connection.Close();
                }
            }catch(Exception ex)
            {
                List<ReportesProdTerm> reportes = new List<ReportesProdTerm>();
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                int line = trace.GetFrame(trace.FrameCount - 1).GetFileLineNumber();
                string Message = string.Empty;
                StringBuilder Mensaje = new StringBuilder();
                Mensaje.Append("Ocurrio un error al enviar el reporte:");
                Mensaje.Append(ex.Message != null ? ex.Message.ToString() : string.Empty);
                Mensaje.Append(ex.InnerException != null ? ex.InnerException.ToString() : string.Empty);
                Mensaje.Append(" line: " + line);

                string rutaEmail = Directory.GetCurrentDirectory() + "/TicketPTM/ErroresStore.txt";
                if (!System.IO.File.Exists(rutaEmail))
                {
                    System.IO.File.Create(rutaEmail).Close();
                }

                using (StreamWriter writer = File.AppendText(rutaEmail))
                {
                    writer.WriteLine($"{DateTime.Now} - {Mensaje.ToString()}");
                    writer.Close();
                }

                return reportes;
            }
			
		}
	}
}
