using AutoMapper;
using DBModel.DB;
using IBussines;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.RequestResponse;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussines
{
    public class CajaBussines : ICajaBussines
    {
        #region Declaracion de vcariables generales
        public readonly ICajaRepository _ICajaRepository = null;
        public readonly IMapper _Mapper;
        public readonly IVentaBussines _IVentaBussines;


        #endregion

        #region constructor 
        public CajaBussines(IMapper mapper)
        {
            _Mapper = mapper;
            _ICajaRepository = new CajaRepository();
        }
        #endregion

        public CajaResponse Create(CajaRequest entity)
        {
            Caja au = _Mapper.Map<Caja>(entity);
            au = _ICajaRepository.Create(au);
            CajaResponse res = _Mapper.Map<CajaResponse>(au);
            return res;
        }

        public List<CajaResponse> CreateMultiple(List<CajaRequest> request)
        {
            List<Caja> au = _Mapper.Map<List<Caja>>(request);
            au = _ICajaRepository.InsertMultiple(au);
            List<CajaResponse> res = _Mapper.Map<List<CajaResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _ICajaRepository.Delete(id);
        }

        public int deleteMultipleItems(List<CajaRequest> request)
        {
            List<Caja> au = _Mapper.Map<List<Caja>>(request);
            int cantidad = _ICajaRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<CajaResponse> getAll()
        {
            List<Caja> lsl = _ICajaRepository.GetAll();
            List<CajaResponse> res = _Mapper.Map<List<CajaResponse>>(lsl);
            return res;
        }

        public List<CajaResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public CajaResponse getById(object id)
        {
            Caja au = _ICajaRepository.GetById(id);
            CajaResponse res = _Mapper.Map<CajaResponse>(au);
            return res;
        }

        public CajaResponse Update(CajaRequest entity)
        {
            // Obtener la caja actual de la base de datos
            Caja cajaActual = _ICajaRepository.GetById(entity.IdCaja);

            if (cajaActual == null)
            {
                throw new Exception("Caja no encontrada.");
            }

            // Mapear los nuevos datos al objeto existente (sin perder campos anteriores)
            cajaActual.SaldoInicial = entity.SaldoInicial;
            cajaActual.SaldoDigital = entity.SaldoDigital;
            cajaActual.IngresosACaja = entity.IngresosACaja;
            cajaActual.Fecha = entity.Fecha;
            cajaActual.FechaCierre = entity.FechaCierre;

            // Acumular el retiro de caja
            if (entity.RetiroDeCaja != 0)
            {
                cajaActual.RetiroDeCaja += entity.RetiroDeCaja;
                cajaActual.SaldoFinal -= entity.RetiroDeCaja;
            }

            // Actualizar caja en base de datos
            Caja updatedCaja = _ICajaRepository.Update(cajaActual);

            if (updatedCaja == null)
            {
                throw new Exception("No se pudo actualizar la caja.");
            }

            // Mapear a response
            CajaResponse res = _Mapper.Map<CajaResponse>(updatedCaja);
            return res;
        }



        public List<CajaResponse> UpdateMultiple(List<CajaRequest> request)
        {
            List<Caja> au = _Mapper.Map<List<Caja>>(request);
            au = _ICajaRepository.UpdateMultiple(au);
            List<CajaResponse> res = _Mapper.Map<List<CajaResponse>>(au);
            return res;
        }

        public Caja RegistrarVentaEnCajaDelDia()
        {
            var cajaDelDia = _ICajaRepository.FindCajaByDate(DateTime.Today);
            if (cajaDelDia == null)
            {
                throw new Exception("No hay una caja abierta para hoy. Por favor, crea una caja primero.");
            }
            return cajaDelDia;
        }




    }
}
