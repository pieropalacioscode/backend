using DBModel.DB;
using Microsoft.AspNetCore.Http;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;

namespace IBussines
{
    public interface IPedidoProveedorBussines : ICRUDBussnies<PedidoProveedorRequest,PedidoProveedorResponse>
    {
        Task<string> CrearPedidoConDetalles(PedidoProveedorConDetalleRequest request);
        Task<string> ConfirmarRecepcionConImagen(int idPedido, int idSucursal, string descripcionRecepcion, List<DetallePedidoProveedorRequest> detalles, List<IFormFile> imagenes);
        Task<List<PedidoProveedorResponse>> getPorEstado(string estado);
        Task<PedidoDetalleResponse?> getPedidoconDetalle(int id);
        Task<PedidoDetalleResponse?> GetPedidoPorFecha(DateTime fecha);
        Task<List<PedidoDetalleResponse>> getPedidoconDetalles(string estado);
    }
}
