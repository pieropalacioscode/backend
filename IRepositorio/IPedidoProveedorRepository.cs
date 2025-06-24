using DBModel.DB;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;
using UtilPaginados;

namespace IRepository
{
    public interface IPedidoProveedorRepository : ICRUDRepositorio<PedidoProveedor>
    {
        Task<List<PedidoProveedor>> getPorEstado(string estado);
        Task<PedidoDetalleResponse?> getPedidoconDetalle(int id);
        Task<PaginacionResponse<PedidoDetalleResponse>> GetPedidosPorFechaPaginado(DateTime fecha, int pagina, int cantidad);
        Task<PaginacionResponse<PedidoDetalleResponse>> GetPedidosConDetallesPaginado(string estado, int pagina, int cantidad);
        Task<ContadorEstadosPedidoResponse> getcanEstado();
    }
}
