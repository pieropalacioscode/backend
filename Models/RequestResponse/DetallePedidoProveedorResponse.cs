namespace Models.RequestResponse
{
    public class DetallePedidoProveedorResponse
    {
        public int Id { get; set; }

        public int IdPedidoProveedor { get; set; }

        public int IdLibro { get; set; }

        public int CantidadPedida { get; set; }

        public int? CantidadRecibida { get; set; }

        public decimal PrecioUnitario { get; set; }
    }
}
