namespace Models.RequestResponse
{
    public class PedidoProveedorRequest
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; } = null!;

        public int IdProveedor { get; set; }
        public string? DescripcionPedido { get; set; }

        public string? DescripcionRecepcion { get; set; }
        public string? Imagen { get; set; }
        public int? IdPersona { get; set; }
    }
}
