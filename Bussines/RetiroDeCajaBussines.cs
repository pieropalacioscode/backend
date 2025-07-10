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
    public class RetiroDeCajaBussines : IRetiroDeCajaBussines
    {
        #region Declaracion de vcariables generales
        public readonly IRetiroDeCajaRepository _IRetiroDeCajaRepository = null;
        public readonly IMapper _Mapper;
        public readonly ICajaRepository _CajaRepository;

        #endregion

        #region constructor 
        public RetiroDeCajaBussines(IMapper mapper, ICajaRepository cajaRepository)
        {
            _Mapper = mapper;
            _IRetiroDeCajaRepository = new RetiroDeCajaRepository();
            _CajaRepository = cajaRepository;
        }

        #endregion

        public RetiroDeCajaResponse Create(RetiroDeCajaRequest entity)
        {
            RetiroDeCaja au = _Mapper.Map<RetiroDeCaja>(entity);
            au = _IRetiroDeCajaRepository.Create(au);
            RetiroDeCajaResponse res = _Mapper.Map<RetiroDeCajaResponse>(au);
            return res;
        }

        public RetiroDeCajaResponse CrearRetiro(RetiroDeCajaRequest request)
        {
            var caja = _CajaRepository.GetById(request.CajaId);
            if (caja == null)
                throw new Exception("Caja no encontrada.");

            if (request.MontoEfectivo < 0 || request.MontoDigital < 0)
                throw new Exception("Los montos deben ser positivos.");

            decimal totalRetiro = request.MontoEfectivo + request.MontoDigital;

            if (caja.SaldoFinal == null || caja.SaldoFinal < totalRetiro)
                throw new Exception("No hay saldo suficiente en la caja para este retiro.");

            // Crear el retiro
            var retiro = new RetiroDeCaja
            {
                CajaId = request.CajaId,
                Descripcion = request.Descripcion,
                Fecha = DateTime.Now,
                MontoEfectivo = request.MontoEfectivo,
                MontoDigital = request.MontoDigital
            };

            _IRetiroDeCajaRepository.Create(retiro);

            // Actualizar caja
            caja.SaldoFinal -= totalRetiro;

            // ✅ Restar también los montos específicos
            caja.IngresosACaja -= request.MontoEfectivo;
            caja.SaldoDigital -= request.MontoDigital;

            _CajaRepository.Update(caja);

            var response = _Mapper.Map<RetiroDeCajaResponse>(retiro);
            return response;
        }


        public List<RetiroDeCajaResponse> CreateMultiple(List<RetiroDeCajaRequest> request)
        {
            List<RetiroDeCaja> au = _Mapper.Map<List<RetiroDeCaja>>(request);
            au = _IRetiroDeCajaRepository.InsertMultiple(au);
            List<RetiroDeCajaResponse> res = _Mapper.Map<List<RetiroDeCajaResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _IRetiroDeCajaRepository.Delete(id);
        }

        public int deleteMultipleItems(List<RetiroDeCajaRequest> request)
        {
            List<RetiroDeCaja> au = _Mapper.Map<List<RetiroDeCaja>>(request);
            int cantidad = _IRetiroDeCajaRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<RetiroDeCajaResponse> getAll()
        {
            List<RetiroDeCaja> lsl = _IRetiroDeCajaRepository.GetAll();
            List<RetiroDeCajaResponse> res = _Mapper.Map<List<RetiroDeCajaResponse>>(lsl);
            return res;
        }

        public List<RetiroDeCajaResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public RetiroDeCajaResponse getById(object id)
        {
            RetiroDeCaja au = _IRetiroDeCajaRepository.GetById(id);
            RetiroDeCajaResponse res = _Mapper.Map<RetiroDeCajaResponse>(au);
            return res;
        }

        public RetiroDeCajaResponse Update(RetiroDeCajaRequest entity)
        {
            RetiroDeCaja au = _Mapper.Map<RetiroDeCaja>(entity);
            au = _IRetiroDeCajaRepository.Update(au);
            RetiroDeCajaResponse res = _Mapper.Map<RetiroDeCajaResponse>(au);
            return res;
        }

        public List<RetiroDeCajaResponse> UpdateMultiple(List<RetiroDeCajaRequest> request)
        {
            List<RetiroDeCaja> au = _Mapper.Map<List<RetiroDeCaja>>(request);
            au = _IRetiroDeCajaRepository.UpdateMultiple(au);
            List<RetiroDeCajaResponse> res = _Mapper.Map<List<RetiroDeCajaResponse>>(au);
            return res;
        }
    }
}
