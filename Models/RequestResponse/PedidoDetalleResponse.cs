using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class PedidoDetalleResponse
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; } = null!;
        public string? DescripcionPedido { get; set; }
        public string? DescripcionRecepcion { get; set; }
        public string Proveedor { get; set; } = null!;
        public int idProveedor { get; set; }
        public string? Imagen { get; set; }
        public int? IdPersona { get; set; }
        public string? NombreCliente { get; set; }
        public List<LibroPedidoDetalleDto> Detalles { get; set; } = new();
    }

    public class LibroPedidoDetalleDto
    {
        public int Id { get; set; }
        public int idLibro { get; set; }
        public string Titulo { get; set; } = null!;
        public string Isbn { get; set; } = null!;
        public string Imagen { get; set; } = null!;
        public int CantidadPedida { get; set; }
        public int? CantidadRecibida { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
