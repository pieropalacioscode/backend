using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class TipoProveedor
{
    public int IdTipoProveedor { get; set; }

    public string? Descripcion { get; set; }
    [JsonIgnore]
    public virtual ICollection<Proveedor> Proveedors { get; set; } = new List<Proveedor>();
}
