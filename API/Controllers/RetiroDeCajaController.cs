using AutoMapper;
using Bussines;
using IBussines;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RetiroDeCajaController : ControllerBase
    {
        #region Declaracion de vcariables generales
        public readonly IRetiroDeCajaBussines _IRetiroDeCajaBussines = null;
        public readonly IMapper _Mapper;
        public readonly ICajaRepository _cajaRepository;
        #endregion

        #region constructor 
        public RetiroDeCajaController(IMapper mapper, ICajaRepository _CajaRepository)
        {
            _Mapper = mapper;
            _cajaRepository = _CajaRepository;
            _IRetiroDeCajaBussines = new RetiroDeCajaBussines(_Mapper,_cajaRepository);

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
            List<RetiroDeCajaResponse> lsl = _IRetiroDeCajaBussines.getAll();
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
            RetiroDeCajaResponse res = _IRetiroDeCajaBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public IActionResult Create([FromBody] RetiroDeCajaRequest request)
        {
            RetiroDeCajaResponse res = _IRetiroDeCajaBussines.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] RetiroDeCajaRequest request)
        {
            RetiroDeCajaResponse res = _IRetiroDeCajaBussines.Update(request);
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
            int res = _IRetiroDeCajaBussines.Delete(id);
            return Ok(res);
        }



        [HttpPost("crear-retiro")]
        public IActionResult CrearRetiro([FromBody] RetiroDeCajaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var retiroResponse = _IRetiroDeCajaBussines.CrearRetiro(request);
                return Ok(retiroResponse);
            }
            catch (Exception ex)
            {
                // Aquí podrías usar un logger para registrar la excepción
                // logger.LogError(ex, "Error al crear retiro de caja");

                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }


        #endregion
    }
}
