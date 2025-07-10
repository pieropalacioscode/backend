using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class PedidoProveedorConDetalleRequest
    {
        public PedidoProveedorRequest Pedido { get; set; }
        public List<DetallePedidoProveedorRequest> Detalles { get; set; }
    }
}
