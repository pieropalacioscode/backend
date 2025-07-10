using AutoMapper;
using DBModel.DB;
using IBussines;
using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Models.RequestResponse;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilPaginados;
using UtilPDF;

namespace Bussines
{
    public class PedidoProveedorBussines : IPedidoProveedorBussines
    {
        #region Declaracion de vcariables generales
        public readonly IPedidoProveedorRepository _IPedidoProveedorRepository ;
        private readonly IDetallePedidoProveedorRepository _detalleRepo;
        private readonly IKardexRepository _kardexRepo;
        public readonly IMapper _Mapper;
        private readonly IFirebaseStorageService _firebaseStorageService;

        #endregion

        #region constructor 
        public PedidoProveedorBussines(IMapper mapper, IDetallePedidoProveedorRepository detalleRepo,
        IKardexRepository kardexRepo, IFirebaseStorageService firebaseStorageService)
        {
            _Mapper = mapper;
            _IPedidoProveedorRepository = new PedidoProveedorRepository();
            _detalleRepo = detalleRepo;
            _kardexRepo = kardexRepo;
            _firebaseStorageService = firebaseStorageService;

        }
        #endregion

        public PedidoProveedorResponse Create(PedidoProveedorRequest entity)
        {
            PedidoProveedor au = _Mapper.Map<PedidoProveedor>(entity);
            au = _IPedidoProveedorRepository.Create(au);
            PedidoProveedorResponse res = _Mapper.Map<PedidoProveedorResponse>(au);
            return res;
        }

        public List<PedidoProveedorResponse> CreateMultiple(List<PedidoProveedorRequest> request)
        {
            List<PedidoProveedor> au = _Mapper.Map<List<PedidoProveedor>>(request);
            au = _IPedidoProveedorRepository.InsertMultiple(au);
            List<PedidoProveedorResponse> res = _Mapper.Map<List<PedidoProveedorResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _IPedidoProveedorRepository.Delete(id);
        }

        public int deleteMultipleItems(List<PedidoProveedorRequest> request)
        {
            List<PedidoProveedor> au = _Mapper.Map<List<PedidoProveedor>>(request);
            int cantidad = _IPedidoProveedorRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<PedidoProveedorResponse> getAll()
        {
            List<PedidoProveedor> lsl = _IPedidoProveedorRepository.GetAll();
            List<PedidoProveedorResponse> res = _Mapper.Map<List<PedidoProveedorResponse>>(lsl);
            return res;
        }

        public List<PedidoProveedorResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public PedidoProveedorResponse getById(object id)
        {
            PedidoProveedor au = _IPedidoProveedorRepository.GetById(id);
            PedidoProveedorResponse res = _Mapper.Map<PedidoProveedorResponse>(au);
            return res;
        }

        public PedidoProveedorResponse Update(PedidoProveedorRequest entity)
        {
            PedidoProveedor au = _Mapper.Map<PedidoProveedor>(entity);
            au = _IPedidoProveedorRepository.Update(au);
            PedidoProveedorResponse res = _Mapper.Map<PedidoProveedorResponse>(au);
            return res;
        }

        public List<PedidoProveedorResponse> UpdateMultiple(List<PedidoProveedorRequest> request)
        {
            List<PedidoProveedor> au = _Mapper.Map<List<PedidoProveedor>>(request);
            au = _IPedidoProveedorRepository.UpdateMultiple(au);
            List<PedidoProveedorResponse> res = _Mapper.Map<List<PedidoProveedorResponse>>(au);
            return res;
        }

        public async Task<string> CrearPedidoConDetalles(PedidoProveedorConDetalleRequest request)
        {
            var pedidoEntity = _Mapper.Map<PedidoProveedor>(request.Pedido);
            pedidoEntity = _IPedidoProveedorRepository.Create(pedidoEntity);

            // Generamos un número de lote único
            string loteGenerado = GenerarCodigoLote();

            foreach (var detalle in request.Detalles)
            {
                var detalleEntity = _Mapper.Map<DetallePedidoProveedor>(detalle);
                detalleEntity.IdPedidoProveedor = pedidoEntity.Id;
                detalleEntity.Lote = loteGenerado; // Asignamos el lote

                _detalleRepo.Create(detalleEntity);
            }

            return "Pedido creado con éxito.";
        }

        private string GenerarCodigoLote()
        {
            return $"LOTE-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        }



        public async Task<string> ConfirmarRecepcionConImagen(
    int idPedido,
    int idSucursal,
    string? descripcionRecepcion,
    List<DetallePedidoProveedorRequest> detalles,
    List<IFormFile> imagenes,
    string estado) // 👈 NUEVO parámetro
        {
            // Subir imágenes a Firebase solo si no está cancelado
            List<string> urlsImagenes = new();
            if (estado != "Cancelado" && imagenes != null && imagenes.Count > 0)
            {
                foreach (var imagen in imagenes)
                {
                    string url = await _firebaseStorageService.UploadPedidosImageAsync(imagen);
                    urlsImagenes.Add(url);
                }
            }

            // Actualizar detalles y registrar en Kardex solo si es "Recibido"
            if (estado == "Recibido")
            {
                foreach (var item in detalles)
                {
                    var detalleExistente = _detalleRepo.GetById(item.Id);
                    if (detalleExistente != null)
                    {
                        detalleExistente.CantidadRecibida = item.CantidadRecibida ?? 0;
                        _detalleRepo.Update(detalleExistente);

                        _kardexRepo.RegistrarEntradaKardex(item.IdLibro, idSucursal, item.CantidadRecibida ?? 0, item.PrecioUnitario);
                    }
                }
            }

            // Actualizar estado del pedido
            var pedido = _IPedidoProveedorRepository.GetById(idPedido);
            pedido.Estado = estado;
            pedido.DescripcionRecepcion = descripcionRecepcion ?? string.Empty;
            if (urlsImagenes.Any())
                pedido.Imagen = string.Join(",", urlsImagenes);

            _IPedidoProveedorRepository.Update(pedido);

            return estado == "Cancelado" ? "Pedido cancelado correctamente." : "Recepción confirmada.";
        }




        public async Task<List<PedidoProveedorResponse>> getPorEstado(string estado)
        {
            var pedidos= await _IPedidoProveedorRepository.getPorEstado(estado);
            var response = pedidos.Select(p => new PedidoProveedorResponse
            {
                Id = p.Id,
                Fecha = p.Fecha,
                Estado = p.Estado,
                DescripcionPedido = p.DescripcionPedido,
                DescripcionRecepcion = p.DescripcionRecepcion
            }).ToList();
            return response;
        }

        public async Task<PedidoDetalleResponse?> getPedidoconDetalle(int id)
        {
           return await _IPedidoProveedorRepository.getPedidoconDetalle(id);  
        }

        public async Task<PaginacionResponse<PedidoDetalleResponse>> GetPedidosPorFechaPaginado(DateTime fecha, int pagina, int cantidad)
        {
            return await _IPedidoProveedorRepository.GetPedidosPorFechaPaginado(fecha,pagina,cantidad);
        }
        public async Task<PaginacionResponse<PedidoDetalleResponse>> getPedidoconDetalles(string estado, int pagina, int cantidad)
        {
            return await _IPedidoProveedorRepository.GetPedidosConDetallesPaginado(estado,pagina,cantidad);

        }

        public async Task<ContadorEstadosPedidoResponse> getcanEstado()
        {
            return await _IPedidoProveedorRepository.getcanEstado();
        }

        public async Task<List<PedidoDetalleResponse>> GetPedidosConDetallesPorFechaYProveedor(DateTime fecha, int idProveedor)
        {
            return await _IPedidoProveedorRepository.GetPedidosConDetallesPorFechaYProveedor(fecha,idProveedor);
        }

        public async Task<byte[]> GenerarPdfPedidosPorFecha(DateTime fecha,int idProveedor)
        {
            var pedidos = await _IPedidoProveedorRepository.GetPedidosConDetallesPorFechaYProveedor(fecha, idProveedor);

            if (pedidos == null || !pedidos.Any())
                throw new Exception("No se encontraron pedidos para esta fecha.");

            // Generar PDF con los pedidos agrupados por proveedor
            var pdfBytes = UtilPDF.PedidoQuestPdfHelper.GenerarPdfPorProveedor(pedidos, fecha);
            return pdfBytes;
        }

        public async Task<string> GenerarYSubirPdfTemporal(DateTime fecha, int idProveedor)
        {
            // 1. Obtener el PDF generado (en bytes)
            var pdfBytes = await GenerarPdfPedidosPorFecha(fecha, idProveedor);

            // 2. Subir a Firebase con eliminación posterior
            var url = await _firebaseStorageService.UploadPedidoPdfTemporalAsync(pdfBytes, $"Pedido_{fecha:yyyyMMdd}");

            return url;
        }

    }
}
