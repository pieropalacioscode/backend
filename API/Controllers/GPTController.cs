using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayPal;
using static Models.RequestResponse.LibroResponse;

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
            var jsonString = await _iaService.ObtenerDatosLibroDesdeGPT(isbn);

            if (jsonString.Contains("Libro no encontrado"))
            {
                return NotFound(new { mensaje = "Libro no encontrado por IA" });
            }

            try
            {
                var objeto = JsonConvert.DeserializeObject<RespuestaLibro>(jsonString);
                return Ok(objeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al deserializar la respuesta de la IA", error = ex.Message });
            }
        }


        [HttpGet("libro/isbn/{isbn}")]
        public async Task<IActionResult> ObtenerPorIsbn(string isbn)
        {
            var resultado = await _iaService.ObtenerLibroDesdeGoogleBooks(isbn);
            return Content(resultado, "application/json");
        }

    }
}
