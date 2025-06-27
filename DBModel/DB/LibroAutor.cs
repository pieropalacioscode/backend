using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class LibroAutor
{
    public int IdLibroAutor { get; set; }

    public int IdAutor { get; set; }

    public int IdLibro { get; set; }
    [JsonIgnore]
    public virtual Autor IdAutorNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Libro IdLibroNavigation { get; set; } = null!;
}
