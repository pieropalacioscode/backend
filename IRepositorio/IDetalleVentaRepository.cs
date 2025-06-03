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
    public interface IDetalleVentaRepository : ICRUDRepositorio<DetalleVenta>
    {
        public Task<IEnumerable<DetalleVenta>> GetDetalleVentasByPersonaId(int idPersona);
        Task<List<ProductosMasVendidosResponse>> ObtenerProductosMasVendidosAsync(int mes, int anio);
        
    }
}
