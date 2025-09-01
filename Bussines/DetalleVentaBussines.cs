using AutoMapper;
using DBModel.DB;
using IBussines;
using IRepository;
using IService;
using Models.RequestResponse;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Repository;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilPDF;

namespace Bussines
{
    public class DetalleVentaBussines : IDetalleVentaBussines
    {
        #region Declaracion de vcariables generales
        public readonly IDetalleVentaRepository _IDetalleVentaRepository;
        public readonly IMapper _Mapper;
        private readonly IMLPredictionService _mlPredictionService;
        private readonly IDimFechaRepository _dimFechaRepository;

        #endregion

        #region constructor 
        public DetalleVentaBussines(IMapper mapper,IMLPredictionService mLPredictionService, IDimFechaRepository dimFechaRepository)
        {
            _Mapper = mapper;
            _IDetalleVentaRepository = new DetalleVentaRepository();
            _mlPredictionService = mLPredictionService;
            _dimFechaRepository = dimFechaRepository;
        }
        #endregion

        public DetalleVentaResponse Create(DetalleVentaRequest entity)
        {
            DetalleVenta au = _Mapper.Map<DetalleVenta>(entity);
            au = _IDetalleVentaRepository.Create(au);
            DetalleVentaResponse res = _Mapper.Map<DetalleVentaResponse>(au);
            return res;
        }

        public List<DetalleVentaResponse> CreateMultiple(List<DetalleVentaRequest> request)
        {
            List<DetalleVenta> au = _Mapper.Map<List<DetalleVenta>>(request);
            au = _IDetalleVentaRepository.InsertMultiple(au);
            List<DetalleVentaResponse> res = _Mapper.Map<List<DetalleVentaResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _IDetalleVentaRepository.Delete(id);
        }

        public int deleteMultipleItems(List<DetalleVentaRequest> request)
        {
            List<DetalleVenta> au = _Mapper.Map<List<DetalleVenta>>(request);
            int cantidad = _IDetalleVentaRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<DetalleVentaResponse> getAll()
        {
            List<DetalleVenta> lsl = _IDetalleVentaRepository.GetAll();
            List<DetalleVentaResponse> res = _Mapper.Map<List<DetalleVentaResponse>>(lsl);
            return res;
        }

        public List<DetalleVentaResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public DetalleVentaResponse getById(object id)
        {
            DetalleVenta au = _IDetalleVentaRepository.GetById(id);
            DetalleVentaResponse res = _Mapper.Map<DetalleVentaResponse>(au);
            return res;
        }

        public DetalleVentaResponse Update(DetalleVentaRequest entity)
        {
            DetalleVenta au = _Mapper.Map<DetalleVenta>(entity);
            au = _IDetalleVentaRepository.Update(au);
            DetalleVentaResponse res = _Mapper.Map<DetalleVentaResponse>(au);
            return res;
        }


        public List<DetalleVentaResponse> UpdateMultiple(List<DetalleVentaRequest> request)
        {
            List<DetalleVenta> au = _Mapper.Map<List<DetalleVenta>>(request);
            au = _IDetalleVentaRepository.UpdateMultiple(au);
            List<DetalleVentaResponse> res = _Mapper.Map<List<DetalleVentaResponse>>(au);
            return res;
        }
        public async Task<IEnumerable<DetalleVenta>> GetDetalleVentasByPersonaId(int idPersona)
        {
            return await _IDetalleVentaRepository.GetDetalleVentasByPersonaId(idPersona);
        }
        public async Task<List<ProductosMasVendidosResponse>> ObtenerProductosMasVendidosAsync(int mes, int anio)
        {
            return await _IDetalleVentaRepository.ObtenerProductosMasVendidosAsync(mes, anio);
        }

        public async Task<List<DetalleVentaResponse>> ObtenerDetallesPorIdsVentasAsync(List<int> idsVentas)
        {
            var detalles = await _IDetalleVentaRepository.ObtenerDetallesPorIdsVentasAsync(idsVentas);
            return detalles.Select(d => new DetalleVentaResponse
            {
                IdDetalleVentas = d.IdDetalleVentas,
                IdLibro = d.IdLibro,
                NombreProducto = d.NombreProducto,
                PrecioUnit = d.PrecioUnit,
                Cantidad = d.Cantidad,
                Importe = d.Importe,
                IdVentas = d.IdVentas,
                Estado = d.Estado,
                Descuento = d.Descuento
            }).ToList();
        }

        public async Task<List<DetalleVentaResponse>> ObtenerDetallesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _IDetalleVentaRepository.ObtenerDetallesPorFechaAsync(fechaInicio, fechaFin);
        }

        public async Task<List<VentaResponsePago>> GetPago()
        {
            return await _IDetalleVentaRepository.GetPago();
        }


        public async Task<List<VentaPrediccionDto>> PredecirVentasAsync(int idLibro, int horizonte = 7)
        {
            // 1. Obtener datos históricos
            var ventasHistoricas = await _IDetalleVentaRepository.GetVentasPorLibroAsync(idLibro);

            if (ventasHistoricas == null || !ventasHistoricas.Any())
                return new List<VentaPrediccionDto>();

            // 2. Obtener DimFecha completa (o solo las fechas necesarias)
            var fechas = await _dimFechaRepository.GetAllAsync(); // Devuelve List<DimFechaDto>

            // 3. Entrenar el modelo con los datos históricos y DimFecha
            var modeloEntrenado = _mlPredictionService.TrainForecastingModel(ventasHistoricas, fechas, horizonte);

            // 4. Generar predicciones usando DimFecha futura
            var ultimaFecha = ventasHistoricas.Max(v => v.Fecha);
            var predicciones = new List<VentaPrediccionDto>();

            for (int i = 1; i <= horizonte; i++)
            {
                var fechaPred = ultimaFecha.AddDays(i);

                var dimFechaPred = fechas.FirstOrDefault(f => f.Fecha.Date == fechaPred.Date);

                var cantidadPredicha = _mlPredictionService.PredictFuture(modeloEntrenado, dimFechaPred);

                predicciones.Add(new VentaPrediccionDto
                {
                    Fecha = fechaPred,
                    CantidadPredicha = cantidadPredicha,
                    DimFecha = dimFechaPred
                });
            }

            return predicciones;
        }


    }
}
