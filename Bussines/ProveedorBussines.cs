using AutoMapper;
using DBModel.DB;
using IBussines;
using IRepository;
using IService;
using Models.RequestResponse;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilPaginados;

namespace Bussines
{
    public class ProveedorBussines: IProveedorBussines
    {
        #region Declaracion de vcariables generales
        public readonly IProveedorRepository _IProveedorRepository = null;
        public readonly IMapper _Mapper;
        public readonly IFirebaseStorageService _firebaseStorageService;
        #endregion

        #region constructor 
        public ProveedorBussines(IMapper mapper,IFirebaseStorageService firebaseStorageService)
        {
            _Mapper = mapper;
            _IProveedorRepository = new ProveedorRepository();
            _firebaseStorageService = firebaseStorageService;
        }
        #endregion

        public ProveedorResponse Create(ProveedorRequest entity)
        {
            Proveedor au = _Mapper.Map<Proveedor>(entity);
            au = _IProveedorRepository.Create(au);
            ProveedorResponse res = _Mapper.Map<ProveedorResponse>(au);
            return res;
        }

        public List<ProveedorResponse> CreateMultiple(List<ProveedorRequest> request)
        {
            List<Proveedor> au = _Mapper.Map<List<Proveedor>>(request);
            au = _IProveedorRepository.InsertMultiple(au);
            List<ProveedorResponse> res = _Mapper.Map<List<ProveedorResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _IProveedorRepository.Delete(id);
        }

        public int deleteMultipleItems(List<ProveedorRequest> request)
        {
            List<Proveedor> au = _Mapper.Map<List<Proveedor>>(request);
            int cantidad = _IProveedorRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<ProveedorResponse> getAll()
        {
            List<Proveedor> lsl = _IProveedorRepository.GetAll();
            List<ProveedorResponse> res = _Mapper.Map<List<ProveedorResponse>>(lsl);
            return res;
        }

        public List<ProveedorResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public ProveedorResponse getById(object id)
        {
            Proveedor au = _IProveedorRepository.GetById(id);
            ProveedorResponse res = _Mapper.Map<ProveedorResponse>(au);
            return res;
        }

        public ProveedorResponse Update(ProveedorRequest entity)
        {
            Proveedor au = _Mapper.Map<Proveedor>(entity);
            au = _IProveedorRepository.Update(au);
            ProveedorResponse res = _Mapper.Map<ProveedorResponse>(au);
            return res;
        }

        public List<ProveedorResponse> UpdateMultiple(List<ProveedorRequest> request)
        {
            List<Proveedor> au = _Mapper.Map<List<Proveedor>>(request);
            au = _IProveedorRepository.UpdateMultiple(au);
            List<ProveedorResponse> res = _Mapper.Map<List<ProveedorResponse>>(au);
            return res;
        }

        public async Task<PaginacionResponse<ProveedorResponseTipo>> ListarPaginadoAsync(int pagina, int cantidad)
        {
            return await _IProveedorRepository.ListarProveedoresConTipoAsync(pagina, cantidad);
        }


        public async Task<PaginacionResponse<ProveedorResponse>> BuscarPaginadoAsync(string nombre, int pagina, int cantidad)
        {
            var resultado = await _IProveedorRepository.BuscarPaginadoAsync(nombre, pagina, cantidad);
            var dtoList = _Mapper.Map<List<ProveedorResponse>>(resultado.Items);

            return new PaginacionResponse<ProveedorResponse>
            {
                Items = dtoList,
                Total = resultado.Total,
                PaginaActual = resultado.PaginaActual,
                TotalPaginas = resultado.TotalPaginas
            };
        }
    }
}
