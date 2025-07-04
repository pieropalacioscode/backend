﻿using DBModel.DB;
using Models.RequestResponse;
using UtilInterface;
using UtilPaginados;

namespace IBussines
{
    public interface IVentaBussines:ICRUDBussnies<VentaRequest, VentaResponse>
    {
        Task<List<DetalleVenta>> GetDetalleVentaByVentaId(int idVenta);
        Task<MemoryStream> CreateVentaPdf(int idVenta);
        Task GenerarYEnviarPdfDeVenta(int idVenta, string emailCliente);
        Task<string> GeneraNumeroComprobante(DatalleCarrito datalle);
        Task<string> GenerarNumeroComprobante();
        Task<IEnumerable<VentaRequest>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        Task<(List<VentaResponse>, int)> GetVentaPaginados(int page, int pageSize);

        Task<(int totalComprobantes, decimal totalComprobantesMonto)> ObtenerResumenVentasAsync();
        Task<List<IngresoMensualResponse>> ObtenerIngresosPorMes(int mes);
        Task<List<TasaRotacionResponse>> ObtenerTasaRotacionInventario(string filtro, int offset, int limit);
        Task<PaginacionResponse<Venta>> GenVentasPaginados(int page, int pageSize);
    }
}
