using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

[Route("api/[controller]")]
[ApiController]
public class BasculasController : ControllerBase
{
    private readonly string connectionString;

    public static class PesoMemoria
    {
        public static ConcurrentDictionary<string, decimal> PesosDiccionario { get; } = new ConcurrentDictionary<string, decimal>();
    }

    public class PesoRequest
    {
        public string IDBascula { get; set; }
        public decimal Peso { get; set; }
    }

    [HttpPost("RecibirPesos")]
    public IActionResult RecibirPesos([FromBody] PesoRequest request)
    {
        try
        {
            PesoMemoria.PesosDiccionario.AddOrUpdate(request.IDBascula, request.Peso, (key, oldValue) => request.Peso);
            return Ok(new { status = 200, message = "Peso recibido y almacenado/actualizado en memoria." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, response = ex.Message });
        }
    }

    [HttpGet("ConsultarPeso/{idBascula}")]
    public IActionResult ConsultarPeso(string idBascula)
    {
        try
        {
            if (PesoMemoria.PesosDiccionario.TryGetValue(idBascula, out var peso))
            {
                return Ok(new { IDBascula = idBascula, Peso = peso });
            }
            else
            {
                return NotFound(new { status = 404, message = "Peso no encontrado para la báscula seleccionada." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al consultar el peso: " + ex.Message });
        }
    }

    [HttpGet("BasculasIDs")]
    public IActionResult BasculasIDs()
    {
        try
        {
            var ids = PesoMemoria.PesosDiccionario.Keys.ToList();
            return Ok(ids);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = 500, message = "Error en el servidor al obtener las básculas: " + ex.Message });
        }
    }

    //nextcontroller
}
