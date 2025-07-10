using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class ConfirmarRecepcionRequest
    {
        public int IdPedido { get; set; }
        public int IdSucursal { get; set; }
        public string? DescripcionRecepcion { get; set; }
        public List<DetallePedidoProveedorRequest> Detalles { get; set; }
    }
}
