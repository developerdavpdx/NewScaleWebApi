using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPtmHana.BLL.Services.AcumuladoEmbServ;

namespace WebApiPtmHana.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcumuladoEmbController : ControllerBase
    {
        private readonly IAcumuladoEmbarque _acumulado;

        public AcumuladoEmbController(IAcumuladoEmbarque acumulado)
        {
            _acumulado = acumulado;
        }

        [HttpPost]
        [Route("AddAcumuladoEmbarque")]
        public async Task<IActionResult> AddAcumuladoEmbarque([FromHeader] string proceso, [FromHeader] string familia, [FromHeader] string peso, [FromHeader] string planta)
        {
            try
            {
                float pesoAE = float.Parse(peso);
                int numplanta = int.Parse(planta);
                bool result = await _acumulado.AgregarAcomuladoEmbarque(proceso, familia, pesoAE, numplanta);

                if (!result)
                    return StatusCode(StatusCodes.Status404NotFound, new { message = "No se ha podido registrar el acumulado de embarque" });

                return StatusCode(StatusCodes.Status200OK, new { message = $"Acumulado Embarque agregado correctamente: Familia {familia}, Proceso {proceso} con un peso {peso}" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { ex.Message });
            }
        }
    }
}
