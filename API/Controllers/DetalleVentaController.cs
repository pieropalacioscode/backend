using AutoMapper;
using Bussines;
using DBModel.DB;
using DocumentFormat.OpenXml.Vml.Office;
using IBussines;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;
using Repository;
using UtilPDF;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]

    //[Authorize]
    public class DetalleVentaController : ControllerBase
    {
        #region Declaracion de vcariables generales
        private readonly IDetalleVentaBussines _detalleVentaBussines;
        private readonly IMapper _Mapper;
        private readonly IKardexRepository _kardexRepository;
        private readonly IKardexBussines _kardexBussines;
        private readonly IDetalleVentaBussines _IDetalleVentaBussines = null;
        private readonly IVentaBussines _IVentaBussines = null;
        private readonly IPersonaBussines _IPersonaBussines;
        private readonly ICajaBussines _ICajaBussines;
        private readonly ICajaRepository _ICajaRepository;
        #endregion

        #region constructor 
        public DetalleVentaController(IDetalleVentaBussines detalleVentaBussines, IMapper mapper, IKardexRepository kardexRepository, IKardexBussines kardexBussines, IVentaBussines ventaBussines, IPersonaBussines personaBussines, ICajaBussines cajaBussines, ICajaRepository iCajaRepository)
        {
            _detalleVentaBussines = detalleVentaBussines;
            _Mapper = mapper;
            _kardexRepository = kardexRepository;
            _kardexBussines = kardexBussines;
            _IDetalleVentaBussines = detalleVentaBussines;
            _IVentaBussines = ventaBussines;
            _IPersonaBussines = personaBussines;
            _ICajaBussines = cajaBussines;
            _ICajaRepository = iCajaRepository;
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
            List<DetalleVentaResponse> lsl = _detalleVentaBussines.getAll();
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
            DetalleVentaResponse res = _detalleVentaBussines.getById(id);
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo registro
        /// </summary>
        /// <param name="request">Registro a insertar</param>
        /// <returns>Retorna el registro insertado</returns>
        [HttpPost]
        public IActionResult Create([FromBody] DetalleVentaRequest request)
        {
            DetalleVentaResponse res = _detalleVentaBussines.Create(request);
            return Ok(res);
        }

        /// <summary>
        /// Actualiza un registro
        /// </summary>
        /// <param name="entity">registro a actualizar</param>
        /// <returns>retorna el registro Actualiza</returns>
        [HttpPut]
        public IActionResult Update([FromBody] DetalleVentaRequest request)
        {
            DetalleVentaResponse res = _detalleVentaBussines.Update(request);
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
            int res = _detalleVentaBussines.Delete(id);
            return Ok(res);
        }

        [HttpGet("traer/{idPersona}")]
        public async Task<IActionResult> GetDetalleVentasByPersonaId(int idPersona)
        {
                var detalleVentas = await _detalleVentaBussines.GetDetalleVentasByPersonaId(idPersona);

                return Ok(detalleVentas);
        }




        //[HttpPost("registrar-venta-detalle")]
        //public async Task<IActionResult> RegistrarVentaYDetalle([FromBody] DatalleCarrito detalleCarrito)
        //{
        //    // Verificar si la Persona con el documento proporcionado ya existe
        //    var personaExistente =  _IPersonaBussines.GetPersonaByDocumento(detalleCarrito.Persona.NumeroDocumento);
        //    int idPersona;  // Solo se declara, no se inicializa aquí.

        //    if (personaExistente == null)
        //    {
        //        // La persona no existe, entonces la creamos
        //        PersonaRequest nuevaPersona = new PersonaRequest
        //        {
        //            Nombre = detalleCarrito.Persona.Nombre,
        //            ApellidoPaterno = detalleCarrito.Persona.ApellidoPaterno,
        //            ApellidoMaterno = detalleCarrito.Persona.ApellidoMaterno,
        //            Correo = detalleCarrito.Persona.Correo,
        //            TipoDocumento = detalleCarrito.Persona.TipoDocumento,
        //            NumeroDocumento = detalleCarrito.Persona.NumeroDocumento,
        //            Telefono = detalleCarrito.Persona.Telefono,
        //        };
        //        var personaCreada = _IPersonaBussines.Create(nuevaPersona);
        //        if (personaCreada == null)
        //        {
        //            return StatusCode(500, "Error al crear la persona");
        //        }
        //        idPersona = personaCreada.IdPersona; // Usamos el ID asignado automáticamente después de crear el registro
        //    }
        //    else
        //    {
        //        idPersona = personaExistente.IdPersona;
        //    }
        //    // Verificar la existencia de una caja para el día actual
        //    var cajaDelDia = _ICajaBussines.RegistrarVentaEnCajaDelDia();
        //    if (cajaDelDia == null)
        //    {
        //        return BadRequest("Es necesario abrir una caja para hoy antes de registrar ventas.");
        //    }
        //    decimal totalVenta = detalleCarrito.Items.Sum(item => item.PrecioVenta * item.Cantidad);

        //    decimal totalPrecio = detalleCarrito.Items.Sum(item => item.PrecioVenta * item.Cantidad);
        //    // Preparación de la entidad Venta con los datos necesarios
        //    VentaRequest ventaRequest = new VentaRequest
        //    {
        //        FechaVenta = DateTime.Now,
        //        TipoComprobante = "Boleta",
        //        IdUsuario = 1, // Suponiendo que este ID viene de la sesión del usuario o es un valor fijo por ahora
        //        NroComprobante = "FAC00", // Este valor podría generarse dinámicamente según tu lógica de negocio
        //        IdPersona = detalleCarrito.Persona.IdPersona, // Asumiendo que el IdCliente viene correctamente desde el front-end
        //        IdCaja = cajaDelDia.IdCaja ,
        //        TotalPrecio = (decimal?)totalPrecio

        //    };

        //    // Intento de creación de la venta en el sistema
        //    var venta = _IVentaBussines.Create(ventaRequest);
        //    if (venta == null)
        //    {
        //        return StatusCode(500, "Error al crear la venta");
        //    }

        //    cajaDelDia.IngresosACaja += totalVenta;
        //    cajaDelDia.SaldoFinal = cajaDelDia.SaldoInicial + cajaDelDia.IngresosACaja;
        //    _ICajaRepository.Update(cajaDelDia);

        //    List<DetalleVentaRequest> listaDetalle = new List<DetalleVentaRequest>();
        //    foreach (var item in detalleCarrito.Items)
        //    {
        //        var kardexActual = _kardexRepository.GetById(item.libro.IdLibro);
        //        if (kardexActual == null || kardexActual.Stock < item.Cantidad)
        //        {
        //            return BadRequest("No hay suficiente stock para el libro con ID " + item.libro.IdLibro);
        //        }
        //        kardexActual.Stock -= item.Cantidad; // Asegúrate de que esto no ponga el stock en negativo
        //        _kardexRepository.Update(kardexActual);
        //        // Aquí, se podría verificar el stock del item
        //        var detalleVentaRequest = new DetalleVentaRequest
        //        {
        //            IdVentas = venta.IdVentas,
        //            NombreProducto = item.libro.Titulo,
        //            PrecioUnit = item.PrecioVenta,
        //            IdLibro = item.libro.IdLibro,
        //            Cantidad = item.Cantidad,
        //            Importe = item.PrecioVenta * item.Cantidad,
        //            Estado = "Pendiente" // Asumiendo un estado inicial para la venta
        //                                 // Agrega aquí más campos si son necesarios
        //        };
        //        listaDetalle.Add(detalleVentaRequest);
        //    }

        //    // Creación de los detalles de venta en el sistema
        //    var detallesCreados = _IDetalleVentaBussines.CreateMultiple(listaDetalle);
        //    if (detallesCreados == null)
        //    {
        //        return StatusCode(500, "Error al crear el detalle de la venta");
        //    }

        //    // Retorno de una respuesta exitosa con un mensaje de confirmación
        //    return Ok(new { Message = "Venta y detalles registrados con éxito" });
        //}


        [HttpPost("registrar-venta-detalle")]
        public async Task<IActionResult> RegistrarVentaYDetalle([FromBody] DatalleCarrito detalleCarrito)
        {
            // 1. Buscar o registrar persona
            var personaExistente = _IPersonaBussines.GetPersonaByDocumento(detalleCarrito.Persona.NumeroDocumento);
            int idPersona;

            if (personaExistente == null)
            {
                var personaCreada = _IPersonaBussines.Create(detalleCarrito.Persona);
                if (personaCreada == null)
                    return StatusCode(500, "Error al crear la persona");

                idPersona = personaCreada.IdPersona;
            }
            else
            {
                idPersona = personaExistente.IdPersona;
            }

            // 2. Verificar que haya una caja abierta hoy
            var cajaDelDia = _ICajaBussines.RegistrarVentaEnCajaDelDia();
            if (cajaDelDia == null)
                return BadRequest("Debe abrir una caja para hoy antes de registrar ventas.");

            // 3. Registrar detalles de los productos y calcular totales
            List<DetalleVentaRequest> listaDetalle = new();
            decimal subtotalProductos = 0;
            decimal totalDescuentosProductos = 0;

            foreach (var item in detalleCarrito.Items)
            {
                var kardexActual = _kardexRepository.GetById(item.libro.IdLibro);
                if (kardexActual == null || kardexActual.Stock < item.Cantidad)
                    return BadRequest($"No hay suficiente stock para el libro con ID {item.libro.IdLibro}");

                kardexActual.Stock -= item.Cantidad;
                _kardexRepository.Update(kardexActual);

                // Calcular subtotal del producto sin descuento
                decimal subtotalProducto = (item.PrecioVenta * item.Cantidad)-item.Descuento;
                subtotalProductos += subtotalProducto;

                // Acumular descuento por producto
                decimal descuentoProducto = item.Descuento;
                totalDescuentosProductos += descuentoProducto;

                // Precio final del producto después del descuento
                decimal precioFinal = item.PrecioVenta;
                decimal importe = (precioFinal * item.Cantidad)-item.Descuento;

                var detalle = new DetalleVentaRequest
                {
                    IdLibro = item.libro.IdLibro,
                    NombreProducto = item.libro.Titulo,
                    PrecioUnit = item.PrecioVenta,
                    Cantidad = item.Cantidad,
                    Importe = importe,
                    Estado = "Pendiente",
                    Descuento = descuentoProducto
                };

                listaDetalle.Add(detalle);
            }

            // 4. Calcular y validar totales
            decimal subtotalDespuesDescuentosProductos = subtotalProductos;
            decimal descuentoVenta = detalleCarrito.descuento ?? 0;

            // Validar que el totalAmount enviado coincida con el calculado (productos con descuentos aplicados)
            if (Math.Round(detalleCarrito.TotalAmount, 2) != Math.Round(subtotalDespuesDescuentosProductos, 2))
            {
                return BadRequest(new
                {
                    message = "El totalAmount no coincide con la suma de productos con descuentos",
                    totalAmountEnviado = detalleCarrito.TotalAmount,
                    totalCalculado = subtotalDespuesDescuentosProductos,
                    subtotalProductos,
                    totalDescuentosProductos,
                    detalle = "totalAmount debe ser: Subtotal productos - Descuentos productos"
                });
            }

            // El total final para cobrar es totalAmount menos el descuento por venta
            decimal totalFinalVenta = detalleCarrito.TotalAmount - descuentoVenta;

            // 5. Validación de consistencia entre montos recibidos y total final de venta
            decimal totalVenta = totalFinalVenta; // El monto real que debe cobrar
            decimal totalRecibido = (detalleCarrito.EfectivoRecibido ?? 0) + (detalleCarrito.MontoDigital ?? 0);
            decimal vuelto = detalleCarrito.vuelto ?? 0;
            decimal totalEsperado = totalRecibido - vuelto;

            if (Math.Round(totalVenta, 2) != Math.Round(totalEsperado, 2))
            {
                return BadRequest(new
                {
                    message = "Inconsistencia en el monto de pago",
                    totalVenta,
                    totalRecibido,
                    vuelto,
                    totalEsperado,
                    diferencia = Math.Round(totalVenta - totalEsperado, 2),
                    detalle = "Total a cobrar = totalAmount - descuento venta"
                });
            }

            // 6. Generar número de comprobante
            string numeroComprobante = await _IVentaBussines.GeneraNumeroComprobante(detalleCarrito);

            // 7. Crear la venta
            var ventaRequest = new VentaRequest
            {
                FechaVenta = DateTime.Now,
                TipoComprobante = detalleCarrito.tipoComprobante,
                IdUsuario = 1,
                NroComprobante = numeroComprobante,
                IdPersona = idPersona,
                IdCaja = cajaDelDia.IdCaja,
                TotalPrecio = totalFinalVenta, // El total final después de todos los descuentos
                TipoPago = detalleCarrito.tipoPago,
                Descuento = descuentoVenta, // Total de descuentos aplicados
                Vuelto = vuelto
            };

            var venta = _IVentaBussines.Create(ventaRequest);
            if (venta == null)
                return StatusCode(500, "Error al crear la venta");

            // 8. Registrar los detalles
            foreach (var d in listaDetalle)
                d.IdVentas = venta.IdVentas;

            var detallesCreados = _IDetalleVentaBussines.CreateMultiple(listaDetalle);
            if (detallesCreados == null)
                return StatusCode(500, "Error al crear los detalles de la venta");

            // 9. Actualizar caja según tipo de pago
            // IMPORTANTE: Se suma el total de la venta (lo que realmente ingresa a caja)
            switch (detalleCarrito.tipoPago?.ToLower())
            {
                case "efectivo":
                    cajaDelDia.IngresosACaja = (cajaDelDia.IngresosACaja ?? 0) + totalVenta;
                    break;

                case "digital":
                    cajaDelDia.SaldoDigital = (cajaDelDia.SaldoDigital ?? 0) + totalVenta;
                    break;

                case "mixto":
                    // En mixto, cada método de pago recibe su parte proporcional del total
                    decimal efectivoRecibido = detalleCarrito.EfectivoRecibido ?? 0;
                    decimal montoDigital = detalleCarrito.MontoDigital ?? 0;

                    // Calcular proporción de cada método de pago (sin contar el vuelto)
                    decimal totalPagado = efectivoRecibido + montoDigital - vuelto;

                    if (totalPagado > 0)
                    {
                        decimal proporcionEfectivo = (efectivoRecibido - vuelto) / totalPagado;
                        decimal proporcionDigital = montoDigital / totalPagado;

                        cajaDelDia.IngresosACaja = (cajaDelDia.IngresosACaja ?? 0) + (totalVenta * proporcionEfectivo);
                        cajaDelDia.SaldoDigital = (cajaDelDia.SaldoDigital ?? 0) + (totalVenta * proporcionDigital);
                    }
                    break;

                default:
                    return BadRequest("Tipo de pago inválido.");
            }

            // 10. Actualizar saldo final de caja
            cajaDelDia.SaldoFinal = (cajaDelDia.SaldoInicial ?? 0)
                                  + (cajaDelDia.IngresosACaja ?? 0)
                                  + (cajaDelDia.SaldoDigital ?? 0);

            _ICajaRepository.Update(cajaDelDia);

            // 11. Respuesta
            return Ok(new
            {
                Message = "Venta y detalles registrados con éxito",
                IdVenta = venta.IdVentas,
                Total = totalFinalVenta,
                TotalAmount = detalleCarrito.TotalAmount, // Suma de productos con descuentos por producto
                Vuelto = vuelto,
                DescuentosTotales = totalDescuentosProductos + descuentoVenta,
                DescuentosProductos = totalDescuentosProductos,
                DescuentoVenta = descuentoVenta,
                SubtotalProductos = subtotalProductos
            });
        }



        [HttpGet("productos-mas-vendidos")]
        public async Task<IActionResult> GetProductosMasVendidos([FromQuery] int mes, [FromQuery] int anio)
        {
            if (mes < 1 || mes > 12 || anio < 2000) // Validación básica
            {
                return BadRequest("Mes o año inválido");
            }

            var productos = await _detalleVentaBussines.ObtenerProductosMasVendidosAsync(mes, anio);
            return Ok(productos);
        }

        [HttpGet("reporte-detalle-ventas")]
        public async Task<IActionResult> GenerarReporteDetalleVentas(DateTime fecha)
        {
            var fechaFin = fecha.AddDays(1);

            var detalles = await _IDetalleVentaBussines.ObtenerDetallesPorFechaAsync(fecha, fechaFin);

            if (!detalles.Any())
                return NotFound("No hay detalles de venta para la fecha seleccionada.");

            var pdfBytes = DetalleVentasPdfHelper.GenerarDetalleVentas(
                detalles,
                vendedor: "ADMIN", // puedes cambiar a usuario logueado si usas JWT
                fechaReporte: fecha,
                filtroFecha: fecha.ToString("dd/MM/yyyy")
            );

            return File(pdfBytes, "application/pdf", $"Detalle_Ventas_{fecha:yyyyMMdd}.pdf");
        }

        [HttpGet("Pagos")]
        public async Task<IActionResult> getPagos()
        {
            var pago= await _IDetalleVentaBussines.GetPago();
            return Ok(pago);
        }

        [HttpGet("predecir-ventas/{idLibro}")]
        public async Task<IActionResult> PredecirVentas(int idLibro, [FromQuery] int horizonte = 7)
        {
            var resultado = await _IDetalleVentaBussines.PredecirVentasAsync(idLibro, horizonte);
            return Ok(resultado);
        }

        #endregion
    }
}
