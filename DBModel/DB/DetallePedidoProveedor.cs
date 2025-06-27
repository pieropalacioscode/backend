using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class DetallePedidoProveedor
{
    public int Id { get; set; }

    public int IdPedidoProveedor { get; set; }

    public int IdLibro { get; set; }

    public int CantidadPedida { get; set; }

    public int? CantidadRecibida { get; set; }

    public decimal PrecioUnitario { get; set; }
    [JsonIgnore]
    public virtual Libro IdLibroNavigation { get; set; } = null!;

    public virtual PedidoProveedor IdPedidoProveedorNavigation { get; set; } = null!;
}
