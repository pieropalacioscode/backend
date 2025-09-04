using DBModel.DB;
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
        Task<List<VentaResponse>> ObtenerVentasPorFecha(DateTime fechaInicio);
        Task<List<VentaResponse>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        Task<(List<VentaResponse>, int)> GetVentaPaginados(int page, int pageSize);

        Task<ResumenDashboardResponse> ObtenerResumenDashboardAsync();
        Task<List<IngresoMensualResponse>> ObtenerIngresosPorMes(int mes);
        Task<List<TasaRotacionResponse>> ObtenerTasaRotacionInventario(string filtro, int offset, int limit);
        Task<PaginacionResponse<Venta>> GenVentasPaginados(int page, int pageSize);
        Task<List<VentaResponse>> getVentasPorComprobante(string NroComprobante);
    }
}
