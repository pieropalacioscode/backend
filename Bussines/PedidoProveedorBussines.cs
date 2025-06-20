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

            foreach (var detalle in request.Detalles)
            {
                var detalleEntity = _Mapper.Map<DetallePedidoProveedor>(detalle);
                detalleEntity.IdPedidoProveedor = pedidoEntity.Id;
                _detalleRepo.Create(detalleEntity); // Usamos tu método sincrónico
            }

            return "Pedido creado con éxito.";
        }


        public async Task<string> ConfirmarRecepcionConImagen(int idPedido, int idSucursal, string descripcionRecepcion, List<DetallePedidoProveedorRequest> detalles, List<IFormFile> imagenes)
        {
            // Subir imágenes a Firebase y obtener URLs
            List<string> urlsImagenes = new();
            foreach (var imagen in imagenes)
            {
                string url = await _firebaseStorageService.UploadPedidosImageAsync(imagen);
                urlsImagenes.Add(url);
            }

            // Continuar con el mismo flujo de ConfirmarRecepcion
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

            var pedido = _IPedidoProveedorRepository.GetById(idPedido);
            pedido.Estado = "Recibido";
            pedido.DescripcionRecepcion = descripcionRecepcion;
            pedido.Imagen = string.Join(",", urlsImagenes); // ✅ Aquí se guarda concatenado
            _IPedidoProveedorRepository.Update(pedido);

            return "Recepción confirmada.";
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

        public async Task<PedidoDetalleResponse?> GetPedidoPorFecha(DateTime fecha)
        {
            return await _IPedidoProveedorRepository.GetPedidoPorFecha(fecha);
        }
        public async Task<List<PedidoDetalleResponse>> getPedidoconDetalles(string estado)
        {
            return await _IPedidoProveedorRepository.getPedidoconDetalles(estado);
        }
    }
}
