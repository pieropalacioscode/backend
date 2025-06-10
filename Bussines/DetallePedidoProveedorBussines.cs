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
    public class DetallePedidoProveedorBussines : IDetallePedidoProveedorBussines
    {
        #region Declaracion de vcariables generales
        public readonly IDetallePedidoProveedorRepository _IDetallePedidoProveedorRepository = null;
        public readonly IMapper _Mapper;

        #endregion

        #region constructor 
        public DetallePedidoProveedorBussines(IMapper mapper)
        {
            _Mapper = mapper;
            _IDetallePedidoProveedorRepository = new DetallePedidoProveedorRepository();
        }
        #endregion

        public DetallePedidoProveedorResponse Create(DetallePedidoProveedorRequest entity)
        {
            DetallePedidoProveedor au = _Mapper.Map<DetallePedidoProveedor>(entity);
            au = _IDetallePedidoProveedorRepository.Create(au);
            DetallePedidoProveedorResponse res = _Mapper.Map<DetallePedidoProveedorResponse>(au);
            return res;
        }

        public List<DetallePedidoProveedorResponse> CreateMultiple(List<DetallePedidoProveedorRequest> request)
        {
            List<DetallePedidoProveedor> au = _Mapper.Map<List<DetallePedidoProveedor>>(request);
            au = _IDetallePedidoProveedorRepository.InsertMultiple(au);
            List<DetallePedidoProveedorResponse> res = _Mapper.Map<List<DetallePedidoProveedorResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _IDetallePedidoProveedorRepository.Delete(id);
        }

        public int deleteMultipleItems(List<DetallePedidoProveedorRequest> request)
        {
            List<DetallePedidoProveedor> au = _Mapper.Map<List<DetallePedidoProveedor>>(request);
            int cantidad = _IDetallePedidoProveedorRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<DetallePedidoProveedorResponse> getAll()
        {
            List<DetallePedidoProveedor> lsl = _IDetallePedidoProveedorRepository.GetAll();
            List<DetallePedidoProveedorResponse> res = _Mapper.Map<List<DetallePedidoProveedorResponse>>(lsl);
            return res;
        }

        public List<DetallePedidoProveedorResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public DetallePedidoProveedorResponse getById(object id)
        {
            DetallePedidoProveedor au = _IDetallePedidoProveedorRepository.GetById(id);
            DetallePedidoProveedorResponse res = _Mapper.Map<DetallePedidoProveedorResponse>(au);
            return res;
        }

        public DetallePedidoProveedorResponse Update(DetallePedidoProveedorRequest entity)
        {
            DetallePedidoProveedor au = _Mapper.Map<DetallePedidoProveedor>(entity);
            au = _IDetallePedidoProveedorRepository.Update(au);
            DetallePedidoProveedorResponse res = _Mapper.Map<DetallePedidoProveedorResponse>(au);
            return res;
        }

        public List<DetallePedidoProveedorResponse> UpdateMultiple(List<DetallePedidoProveedorRequest> request)
        {
            List<DetallePedidoProveedor> au = _Mapper.Map<List<DetallePedidoProveedor>>(request);
            au = _IDetallePedidoProveedorRepository.UpdateMultiple(au);
            List<DetallePedidoProveedorResponse> res = _Mapper.Map<List<DetallePedidoProveedorResponse>>(au);
            return res;
        }
    }
}
