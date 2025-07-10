using Models.Shared.Hubs;
using AutoMapper;
using Bussines;
using IBussines;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.RequestResponse;
using IRepository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionController : ControllerBase
    {
        #region Declaracion de vcariables generales
        private readonly INotificacionBussines _INotificacionBussines;
        private readonly IMapper _Mapper;
        private readonly IHubContext<NotificacionHub> _hubContext;
        #endregion

        #region constructor 
        public NotificacionController(
            INotificacionBussines notificacionBussines,
            IMapper mapper,
            IHubContext<NotificacionHub> hubContext)
        {
            _INotificacionBussines = notificacionBussines;
            _Mapper = mapper;
            _hubContext = hubContext;
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
            List<NotificacionResponse> lsl = _INotificacionBussines.getAll();
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
            NotificacionResponse res = _INotificacionBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public IActionResult Create([FromBody] NotificacionRequest request)
        {
            NotificacionResponse res = _INotificacionBussines.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] NotificacionRequest request)
        {
            NotificacionResponse res = _INotificacionBussines.Update(request);
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
            int res = _INotificacionBussines.Delete(id);
            return Ok(res);
        }

        [HttpPost("crear-y-notificar")]
        public async Task<IActionResult> CreateYNotificar([FromBody] NotificacionRequest request)
        {
            var result = _INotificacionBussines.Create(request);

            // 🔔 Notificar en tiempo real
            await _hubContext.Clients.All.SendAsync("RecibirNotificacion", result);

            return Ok(result);
        }

        [HttpGet("verificar-stock")]
        public async Task<IActionResult> VerificarStock()
        {
            await _INotificacionBussines.VerificarStockBajoYNotificar();
            return Ok(new { success = true, message = "Stock verificado." });
        }

        #endregion
    }
}
