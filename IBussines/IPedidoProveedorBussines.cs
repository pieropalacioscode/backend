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
    public interface IPedidoProveedorBussines : ICRUDBussnies<PedidoProveedorRequest,PedidoProveedorResponse>
    {
        Task<string> CrearPedidoConDetalles(PedidoProveedorConDetalleRequest request);
        Task<string> ConfirmarRecepcion(int idPedido, int idSucursal,string DescripcionRecepcion, List<DetallePedidoProveedorRequest> detalles);
        Task<List<PedidoProveedorResponse>> getPorEstado(string estado);
        Task<PedidoDetalleResponse?> getPedidoconDetalle(int id);
    }
}
