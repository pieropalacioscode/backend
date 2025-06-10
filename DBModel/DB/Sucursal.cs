using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public string? Ubicacion { get; set; }
    [JsonIgnore]
    public virtual ICollection<DocSalida> DocSalida { get; set; } = new List<DocSalida>();
    [JsonIgnore]
    public virtual ICollection<Kardex> Kardices { get; set; } = new List<Kardex>();
}
