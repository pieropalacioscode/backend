using AutoMapper;
using Bussines;
using IBussines;
using IService;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]

    //[Authorize]
    public class ProveedorController : ControllerBase
    {
        #region Declaracion de vcariables generales
        public readonly IProveedorBussines _IProveedorBussines = null;
        public readonly IMapper _Mapper;
        public readonly IFirebaseStorageService _firebaseStorageService;
        #endregion

        #region constructor 
        public ProveedorController(IMapper mapper,IFirebaseStorageService firebaseStorageService)
        {
            _Mapper = mapper;
            _firebaseStorageService = firebaseStorageService;
            _IProveedorBussines = new ProveedorBussines(_Mapper,_firebaseStorageService);
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
            List<ProveedorResponse> lsl = _IProveedorBussines.getAll();
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
            ProveedorResponse res = _IProveedorBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public IActionResult Create([FromBody] ProveedorRequest request)
        {
            ProveedorResponse res = _IProveedorBussines.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] ProveedorRequest request)
        {
            ProveedorResponse res = _IProveedorBussines.Update(request);
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
            int res = _IProveedorBussines.Delete(id);
            return Ok(res);
        }


        [HttpGet("paginado")]
        public async Task<IActionResult> ListarConTipo(int pagina = 1, int cantidad = 10)
        {
            var result = await _IProveedorBussines.ListarPaginadoAsync(pagina, cantidad);
            return Ok(result);
        }


        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string nombre, [FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            var data = await _IProveedorBussines.BuscarPaginadoAsync(nombre, pagina, cantidad);
            return Ok(data);
        }
        #endregion
    }
}
