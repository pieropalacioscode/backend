namespace Models.RequestResponse
{
    public class PedidoProveedorResponse
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; } = null!;

        public int IdProveedor { get; set; }
        public string? DescripcionPedido { get; set; }

        public string? DescripcionRecepcion { get; set; }
        public string? Imagen { get; set; }
    }

    public class ConfirmarRecepcionJsonRequest
    {
        public int IdPedido { get; set; }
        public int IdSucursal { get; set; }
        public string? DescripcionRecepcion { get; set; }
        public List<DetallePedidoProveedorRequest> Detalles { get; set; }
        public List<string>? ImagenesBase64 { get; set; }
        public string? Estado { get; set; }
    }
}
