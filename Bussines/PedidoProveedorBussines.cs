using AutoMapper;
using DBModel.DB;
using IBussines;
using IRepository;
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

        #endregion

        #region constructor 
        public PedidoProveedorBussines(IMapper mapper, IDetallePedidoProveedorRepository detalleRepo,
        IKardexRepository kardexRepo)
        {
            _Mapper = mapper;
            _IPedidoProveedorRepository = new PedidoProveedorRepository();
            _detalleRepo = detalleRepo;
            _kardexRepo = kardexRepo;

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


        public async Task<string> ConfirmarRecepcion(int idPedido, int idSucursal, string DescripcionRecepcion, List<DetallePedidoProveedorRequest> detalles)
        {
            foreach (var item in detalles)
            {
                var detalleExistente = _detalleRepo.GetById(item.Id);
                if (detalleExistente != null)
                {
                    detalleExistente.CantidadRecibida = item.CantidadRecibida ?? 0;
                    _detalleRepo.Update(detalleExistente);

                    // Aumentar stock (crear este método en KardexRepository)
                    _kardexRepo.RegistrarEntradaKardex(item.IdLibro, idSucursal, item.CantidadRecibida ?? 0, item.PrecioUnitario);
                }
            }

            var pedido = _IPedidoProveedorRepository.GetById(idPedido);
            pedido.Estado = "Completado";
            pedido.DescripcionRecepcion = DescripcionRecepcion;
            _IPedidoProveedorRepository.Update(pedido);

            return "Recepción confirmada.";
        }




    }
}
