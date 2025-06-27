using AutoMapper;
using Bussines;
using DBModel.DB;

//using Bussines;
using IBussines;
using IService;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;
using Service;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class LibroController : ControllerBase
    {
        #region Declaracion de vcariables generales
        public readonly ILibroBussines _ILibroBussines = null;
        public readonly IMapper _Mapper;


        private readonly IConfiguration _configuration;
        #endregion

        #region constructor 
        public LibroController(IMapper mapper, ILibroBussines libroBussines)
        {
            _Mapper = mapper;
            _ILibroBussines = libroBussines;
            
        }
        #endregion

        #region crud methods
        /// <summary>
        /// Retorna todos los registros
        /// </summary>
        /// <returns>Retorna todos los registros</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            List<LibroResponse> lsl = _ILibroBussines.getAll();
            return Ok(lsl);
        }




        /// <summary>
        /// retorna el registro por Primary key
        /// </summary>
        /// <param name="id">PK</param>
        /// <returns>retorna el registro</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            LibroResponse res = _ILibroBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] LibroRequest request, IFormFile imageFile = null)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await _ILibroBussines.CreateWithImage(request, imageFile);
                    return Ok(imageUrl);
                }
                else
                {
                    var libroResponse = _ILibroBussines.Create(request);
                    return Ok(libroResponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }




        //[HttpPost]
        //public IActionResult Create([FromBody] LibroRequest request)
        //{
        //    LibroResponse res = _ILibroBussines.Create(request);
        //    return Ok(res);
        //}
        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] LibroRequest request)
        {
            LibroResponse res = _ILibroBussines.Update(request);
            return Ok(res);
        }

        /// <summary>
        /// Elimina un registro
        /// </summary>
        /// <param name="id">Valor del PK</param>
        /// <returns>Cantidad de registros afectados</returns>
        [HttpDelete("{id}")]
        public IActionResult delete(int id)
        {
            int res = _ILibroBussines.Delete(id);
            return Ok(res);
        }

        [HttpGet("{libroId}/precios-objetivos")]
        public async Task<ActionResult<Libro>> ObtenerLibroConPreciosYPublicoObjetivo(int libroId)
        {
            // Obtener el libro con sus precios y el público objetivo asociado, devolviendo solo los IDs.
            var libroConIds = await _ILibroBussines.ObtenerLibroConPreciosYPublicoObjetivo(libroId);

            if (libroConIds == null)
            {
                return NotFound();
            }

            var libroCompleto = await _ILibroBussines.ObtenerLibroCompletoPorIds(libroConIds);

            if (libroCompleto == null)
            {
                return NotFound();
            }

            return Ok(libroCompleto);
        }


        [HttpGet("precios/{libroId}")]
        public async Task<IActionResult> GetPreciosByLibroId(int libroId)
        {
            // Obtener precios del servicio de negocios
            var precios = await _ILibroBussines.GetPreciosByLibroId(libroId);

            // Verificar si hay precios y devolver la lista de precios o una lista vacía
            if (precios != null)
            {
                return Ok(precios);
            }
            else
            {
                return Ok(new List<Precio>()); // Devolver una lista vacía en lugar de un código de error
            }
        }

        [HttpGet("kardex/{libroId}")]
        public async Task<IActionResult> GetKardexByLibroId(int libroId)
        {
            var kardex = await _ILibroBussines.GetKardexByLibroId(libroId);
            if (kardex != null)
            {
                return Ok(kardex);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("Paginator")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            try
            {
                var (libroResponses, totalItems) = await _ILibroBussines.GetLibrosPaginados(page, pageSize);
                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    Libros = libroResponses
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("filtroComplete")]
        public async Task<IActionResult> Autocomplete(string titulo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titulo))
                {
                    return BadRequest("La consulta de búsqueda no puede estar vacía.");
                }

                var results = await _ILibroBussines.filtroComplete(titulo);
                // En lugar de retornar un NotFound si no hay resultados, simplemente retorna una lista vacía
                return Ok(results ?? new List<LibroResponse>());
            }
            catch (Exception ex)
            {
                // Es una buena práctica manejar excepciones inesperadas para evitar que la app se caiga y dar una respuesta al cliente.
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        [HttpGet("Paginator/detalles")]
        public async Task<IActionResult> GetLibrosConDetallePaginadoAsync([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            if (pagina <= 0 || cantidad <= 0)
                return BadRequest(new { success = false, message = "Parámetros de paginación inválidos" });

            var result = await _ILibroBussines.GetLibrosConDetallePaginadoAsync(pagina, cantidad);
            return Ok(new
            {
                success = true,
                data = result.Items,
                total = result.Total,
                paginaActual = result.PaginaActual,
                totalPaginas = result.TotalPaginas
            });
        }


        [HttpGet("inventario")]
        public async Task<IActionResult> GetInventarioPaginado([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            var resultado = await _ILibroBussines.GetInventarioPaginadoAsync(pagina, cantidad);
            return Ok(resultado);
        }

        [HttpGet("buscar-inventario")]
        #endregion
        public async Task<IActionResult>  BuscarEnInventario(string query)
        {
            var datos = await _ILibroBussines.BuscarEnInventario(query);
            return Ok(datos);
        }

        /// <summary>
        /// Inserta un nuevo registro con la opción de subir un archivo a Firebase
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <param name="imageFile">Archivo de imagen a subir</param>
        /// <returns>Retorna el registro insertado con la URL de la imagen</returns>
        [HttpPost("createDetalleLibro")]
        public async Task<IActionResult> CreateWithImageFirebase([FromForm] LibroconautorRequest request, decimal precioVenta, int stock, IFormFile? imageFile = null)
        {
            try
            {
                var libroResponse = await _ILibroBussines.CreateImagenDetalle(request, imageFile, precioVenta, stock);
                return Ok(libroResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }


        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro actualizado</returns>
        [HttpPut("detalles")]
        public async Task<IActionResult> Update([FromForm] LibroconautorRequest request, IFormFile? imageFile, decimal precioVenta, int stock)
        {
            // Esperamos a que la tarea se complete
            LibroResponse res = await _ILibroBussines.UpdateLib(request, imageFile, precioVenta, stock);
            return Ok(res);
        }

        //Filtrador Completo
        [HttpGet("filtrar")]
        public async Task<IActionResult> FiltrarLibros([FromQuery] bool? estado, [FromQuery] string? titulo = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (libros, totalItems) = await _ILibroBussines.FiltrarLibrosAsync(estado, titulo, page, pageSize);

            return Ok(new { libros, totalItems });
        }
        //Cambiar estado a inactivo
        [HttpPut("cambiar-estado/{id}")]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var resultado = await _ILibroBussines.CambiarEstadoLibro(id);
            if (!resultado)
            {
                return NotFound(new { mensaje = "Libro no encontrado" });
            }

            return Ok(new { mensaje = "Estado actualizado correctamente" });
        }
    }
}
    


