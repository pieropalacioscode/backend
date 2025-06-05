﻿using AutoMapper;
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
            // Mapear CajaRequest a Caja (entidad)
            Caja cajaToUpdate = _Mapper.Map<Caja>(entity);

            // Llamar al método Update del repositorio
            Caja updatedCaja = _ICajaRepository.Update(cajaToUpdate);

            // Comprobar si la caja se ha actualizado correctamente
            if (updatedCaja == null)
            {
                // Manejar el caso en que la caja no se actualizó correctamente
                // Puedes lanzar una excepción o devolver un valor que indique el fallo
            }

            // Mapear la caja actualizada de nuevo a CajaResponse
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
