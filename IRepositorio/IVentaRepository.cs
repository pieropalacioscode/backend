using DBModel.DB;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;

namespace IRepository
{
    public interface IVentaRepository : ICRUDRepositorio<Venta>
    {
        Task<List<DetalleVenta>> GetDetallesByVentaId(int idVenta);
        Task<(Venta venta, List<DetalleVenta> detalles)> GetVentaConDetalles(int idVenta);
        Task<Persona> GetPersonaByVentaId(int idVenta);
        Task<string> ObtenerUltimoNumeroComprobantePorTipo(string prefijo);
        Task<IEnumerable<Venta>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<(List<Venta>, int)> GetVentaPaginados(int page, int pageSize);
        Task<(int totalComprobantes, decimal montoTotalComprobantes)> ObtenerResumenDashboardAsync();
        Task<List<IngresoMensualResponse>> ObtenerIngresosPorMes(int mes);
        Task<List<TasaRotacionResponse>> ObtenerTasaRotacionInventario(string filtro, int offset, int limit);
    }
}
