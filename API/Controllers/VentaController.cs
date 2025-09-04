using AutoMapper;
using Bussines;
using IBussines;
using IBussnies;
using IService;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;
using System.Globalization;
using UtilPDF;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[Authorize]
    public class VentaController : ControllerBase
    {
        #region Declaracion de vcariables generales
        public readonly IVentaBussines _IVentaBussines = null;
        public readonly IMapper _Mapper;
        public readonly IEmailService _emailService;
        public readonly IDetalleVentaBussines _IDetalleVentaBussines;
        #endregion

        #region constructor 
        public VentaController(IMapper mapper,IEmailService emailService, IDetalleVentaBussines iDetalleVentaBussines)
        {
            _Mapper = mapper;
            _emailService = emailService;
            _IVentaBussines = new VentaBussines(_Mapper, _emailService);
            _IDetalleVentaBussines = iDetalleVentaBussines;
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
            List<VentaResponse> lsl = _IVentaBussines.getAll();
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
            VentaResponse res = _IVentaBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public IActionResult Create([FromBody] VentaRequest request)
        {
            VentaResponse res = _IVentaBussines.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] VentaRequest request)
        {
            VentaResponse res = _IVentaBussines.Update(request);
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
            int res = _IVentaBussines.Delete(id);
            return Ok(res);
        }


        [HttpGet("{idVenta}/detallesVenta")]
        public async Task<IActionResult> GetDetalleVentaByVentaId(int idVenta)
        {
            var detallesVenta = await _IVentaBussines.GetDetalleVentaByVentaId(idVenta);
            if (detallesVenta == null || detallesVenta.Count == 0)
            {
                return NotFound("No se encontraron detalles para la venta especificada.");
            }
            return Ok(detallesVenta);
        }

        [HttpGet("{idVenta}/pdf")]
        public async Task<IActionResult> GetVentaPdf(int idVenta)
        {
            try
            {
                MemoryStream pdfStream = await _IVentaBussines.CreateVentaPdf(idVenta);
                if (pdfStream == null || pdfStream.Length == 0)
                {
                    return NotFound("PDF no pudo ser generado.");
                }

                // Devuelve el archivo PDF.
                string filename = $"Venta_{idVenta}.pdf";
                return File(pdfStream, "application/pdf", filename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ObtenerPorFechas")]
        public async Task<IActionResult> ObtenerPorFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var ventas = await _IVentaBussines.ObtenerVentasPorFechaAsync(fechaInicio, fechaFin);
            if (!ventas.Any())
            {
                return NotFound("No se encontraron ventas en el rango de fechas especificado.");
            }
            return Ok(ventas);
        }


        [HttpGet("Paginator")]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            try
            {
                var (ventaResponse, totalItems) = await _IVentaBussines.GetVentaPaginados(page, pageSize);
                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    Venta = ventaResponse
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("resumen-ventas")]
        public async Task<IActionResult> ObtenerResumenVentas()
        {
            var resumen = await _IVentaBussines.ObtenerResumenDashboardAsync();

            return Ok(new
            {
                    TotalComprobantes = resumen.TotalComprobantes,
                    MontoTotal = $"S/ {resumen.MontoTotalComprobantes:N2}",

                    TotalBoletas = resumen.TotalBoletas,
                    montoTotalBoletas = $"S/ {resumen.MontoBoletas:N2}",

                    TotalFacturas = resumen.TotalFacturas,
                    MontoTotalFacturas = $"S/ {resumen.MontoFacturas:N2}",

                    TotalNotas = resumen.TotalNotas,
                    MontoTotalNotas = $"S/ {resumen.MontoNotas:N2}",

            });
        }

        [HttpGet("ingresos-por-mes")]
        public async Task<IActionResult> ObtenerIngresosPorMes([FromQuery] int mes)
        {
            var ingresos = await _IVentaBussines.ObtenerIngresosPorMes(mes);
            return Ok(ingresos);
        }

        [HttpGet("tasaRotacion")]
        public async Task<IActionResult> ObtenerTasaRotacionInventario(
            [FromQuery] string filtro ,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10)
        {
            var tasa = await _IVentaBussines.ObtenerTasaRotacionInventario(filtro, offset, limit);
            return Ok(new
            {
                Tasa = tasa
            });
        }

        [HttpGet("paginado")]
        public async Task<IActionResult> GetVentasPaginadas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var paginacion = await _IVentaBussines.GenVentasPaginados(page, pageSize);
            return Ok(paginacion);
        }


        [HttpGet("resumen-ingresos")]
        public async Task<IActionResult> GenerarResumenIngresos(string fecha)
        {
            if (!DateTime.TryParseExact(fecha, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaParsed))
                return BadRequest("Fecha inválida");

            var fechaInicio = fechaParsed.Date;

            var ventas = await _IVentaBussines.ObtenerVentasPorFecha(fechaInicio);

            if (!ventas.Any())
                return NotFound("No hay ventas para la fecha seleccionada.");

            var idsVentas = ventas.Select(v => v.IdVentas).ToList();

            var detalles = await _IDetalleVentaBussines.ObtenerDetallesPorIdsVentasAsync(idsVentas);

            var detallesPorVenta = detalles
                .GroupBy(d => d.IdVentas ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            var pdf = ResumenIngresoPdfHelper.GenerarResumenIngresos(
                ventas,
                detallesPorVenta,
                vendedor: "",
                fechaReporte: fechaParsed
            );

            return File(pdf, "application/pdf", $"Resumen_Ingresos_{fecha:yyyyMMdd}.pdf");
        }

        [HttpGet("nroComprobante/{nroComprobante}")]
        public async Task<IActionResult> getVentasporComprobante(string nroComprobante)
        {
            var ventas = await _IVentaBussines.getVentasPorComprobante(nroComprobante);
            return Ok(ventas);
        }

        #endregion
    }
}
