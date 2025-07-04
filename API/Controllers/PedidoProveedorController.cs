﻿using AutoMapper;
using Bussines;
using DBModel.DB;
using IBussines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
        public async Task<IActionResult> CrearPedidoConDetalles([FromBody] PedidoProveedorConDetalleRequest request)
        {
            try
            {
                var resultado = await _IPedidoProveedorBussines.CrearPedidoConDetalles(request);
                return Ok(new { success = true, message = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPut("confirmar-recepcion-con-imagen")]
        public async Task<IActionResult> ConfirmarRecepcionConImagen(
            [FromForm] int idPedido,
            [FromForm] int idSucursal,
            [FromForm] string? descripcionRecepcion,
            [FromForm] string detallesJson,
            [FromForm] List<IFormFile> imagenes,
            [FromForm] string? estado)
        {
            try
            {
                // 1. Deserializar los detalles desde JSON
                var detalles = JsonConvert.DeserializeObject<List<DetallePedidoProveedorRequest>>(detallesJson);

                // 2. Enviar a la capa de negocio
                var mensaje = await _IPedidoProveedorBussines.ConfirmarRecepcionConImagen(
                    idPedido,
                    idSucursal,
                    descripcionRecepcion,
                    detalles,
                    imagenes,
                    estado
                    
                );

                return Ok(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }




        [HttpGet("estado")]
        public async Task<IActionResult> getPorEstado(string estado)
        {
            var lsl= await _IPedidoProveedorBussines.getPorEstado(estado);
            return Ok(lsl);

        }


        [HttpGet("con-detalles/{id}")]
        public async Task<IActionResult> GetPedidoConDetalle(int id)
        {
            var pedido = await _IPedidoProveedorBussines.getPedidoconDetalle(id);
            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        [HttpGet("fecha")]
        public async Task<IActionResult> GetPedidosFecha([FromQuery] DateTime fecha, [FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            var fechas = await _IPedidoProveedorBussines.GetPedidosPorFechaPaginado(fecha,pagina,cantidad);
            if(fechas==null)
            {
                
                return NotFound();
            }
            return Ok(fechas);
        }


        [HttpGet("Detalles/estado")]
        public async Task<IActionResult> getPedidoconDetalles([FromQuery] string estado, [FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            var detalles = await _IPedidoProveedorBussines.getPedidoconDetalles(estado,pagina,cantidad);
            if (detalles == null)
            {
                
                return NotFound();
            }
            return Ok(detalles);
        }

        [HttpGet("contador")]
        public async Task<IActionResult> GetCant()
        {
            var catidad= await _IPedidoProveedorBussines.getcanEstado();
            return Ok(catidad);
        }
        #endregion
    }
}
