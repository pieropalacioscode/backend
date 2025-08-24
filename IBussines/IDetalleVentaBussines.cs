using DBModel.DB;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;

namespace IBussines
{
    public interface IDetalleVentaBussines : ICRUDBussnies<DetalleVentaRequest,DetalleVentaResponse>
    {
        Task<IEnumerable<DetalleVenta>> GetDetalleVentasByPersonaId(int idPersona);
        Task<List<ProductosMasVendidosResponse>> ObtenerProductosMasVendidosAsync(int mes, int anio);
        Task<List<DetalleVentaResponse>> ObtenerDetallesPorIdsVentasAsync(List<int> idsVentas);
        Task<List<DetalleVentaResponse>> ObtenerDetallesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<VentaResponsePago>> GetPago();
        Task<List<VentaPrediccionDto>> PredecirVentasAsync(int idLibro, int horizonte = 7);
    }
}
