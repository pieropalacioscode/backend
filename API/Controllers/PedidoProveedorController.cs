using AutoMapper;
using Bussines;
using IBussines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoProveedorController : ControllerBase
    {
        #region Declaracion de vcariables generales
        public readonly IPedidoProveedorBussines _IPedidoProveedorBussines = null;
        public readonly IMapper _Mapper;
        public readonly IDetallePedidoProveedorBussines _detallePedidoProveedorBussines;
        public readonly IKardexBussines _kardexBussines;
        #endregion

        #region constructor 
        public PedidoProveedorController(
            IPedidoProveedorBussines pedidoProveedorBussines,
            IMapper mapper,
            IDetallePedidoProveedorBussines detallePedidoProveedorBussines,
            IKardexBussines kardexBussines)
        {
            _IPedidoProveedorBussines = pedidoProveedorBussines;
            _Mapper = mapper;
            _detallePedidoProveedorBussines = detallePedidoProveedorBussines;
            _kardexBussines = kardexBussines;
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
            List<PedidoProveedorResponse> lsl = _IPedidoProveedorBussines.getAll();
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
            PedidoProveedorResponse res = _IPedidoProveedorBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public IActionResult Create([FromBody] PedidoProveedorRequest request)
        {
            PedidoProveedorResponse res = _IPedidoProveedorBussines.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] PedidoProveedorRequest request)
        {
            PedidoProveedorResponse res = _IPedidoProveedorBussines.Update(request);
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
            int res = _IPedidoProveedorBussines.Delete(id);
            return Ok(res);
        } 

        [HttpPost("create-with-details")]
        public IActionResult CrearPedidoConDetalles([FromBody] PedidoProveedorConDetalleRequest request)
        {
            try
            {
                var pedidoCreado = _IPedidoProveedorBussines.Create(request.Pedido);
                
                foreach (var detalle in request.Detalles)
                {
                    detalle.IdPedidoProveedor = pedidoCreado.Id; // asignar FK
                    _detallePedidoProveedorBussines.Create(detalle);
                }

                return Ok(new { success = true, data = pedidoCreado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("confirmar-recepcion")]
        public async Task<IActionResult> ConfirmarRecepcion([FromBody] ConfirmarRecepcionRequest request)
        {
            try
            {
                var resultado = await _IPedidoProveedorBussines.ConfirmarRecepcion(
                    request.IdPedido,
                    request.IdSucursal,
                    request.DescripcionRecepcion,
                    request.Detalles
                );
                return Ok(new { success = true, message = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}
