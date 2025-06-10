using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string? RazonSocial { get; set; }

    public string? Ruc { get; set; }

    public string? Direccion { get; set; }

    public int IdTipoProveedor { get; set; }

    public virtual TipoProveedor IdTipoProveedorNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
    [JsonIgnore]
    public virtual ICollection<PedidoProveedor> PedidoProveedors { get; set; } = new List<PedidoProveedor>();
}
