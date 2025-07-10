using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayPal;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly IGPTIAservice _iaService;

        public GPTController(IGPTIAservice gPTIAservice)
        {
            _iaService = gPTIAservice;
        }

        [HttpGet("libro")]
        public async Task<IActionResult> ObtenerLibroPorISBN([FromQuery] string isbn)
        {
            var respuesta = await _iaService.ObtenerDatosLibroDesdeGPT(isbn);

            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return StatusCode(500, "Error: OpenAI no devolvió respuesta válida");
            }

            return Content(respuesta, "application/json");
        }

    }
}
