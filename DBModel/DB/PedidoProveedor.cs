using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class PedidoProveedor
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public int IdProveedor { get; set; }

    public string? DescripcionPedido { get; set; }

    public string? DescripcionRecepcion { get; set; }

    public string? Imagen { get; set; }

    public virtual ICollection<DetallePedidoProveedor> DetallePedidoProveedors { get; set; } = new List<DetallePedidoProveedor>();
    [JsonIgnore]
    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
