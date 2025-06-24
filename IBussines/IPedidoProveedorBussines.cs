using DBModel.DB;
using Microsoft.AspNetCore.Http;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;
using UtilPaginados;

namespace IBussines
{
    public interface IPedidoProveedorBussines : ICRUDBussnies<PedidoProveedorRequest,PedidoProveedorResponse>
    {
        Task<string> CrearPedidoConDetalles(PedidoProveedorConDetalleRequest request);
        Task<string> ConfirmarRecepcionConImagen(int idPedido,int idSucursal,string? descripcionRecepcion,List<DetallePedidoProveedorRequest> detalles,List<IFormFile> imagenes,string estado);
        Task<List<PedidoProveedorResponse>> getPorEstado(string estado);
        Task<PedidoDetalleResponse?> getPedidoconDetalle(int id);
        Task<PaginacionResponse<PedidoDetalleResponse>> GetPedidosPorFechaPaginado(DateTime fecha, int pagina, int cantidad);
        Task<PaginacionResponse<PedidoDetalleResponse>> getPedidoconDetalles(string estado, int pagina, int cantidad);

        Task<ContadorEstadosPedidoResponse> getcanEstado();
    }
}
