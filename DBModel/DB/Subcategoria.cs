﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class Subcategoria
{
    public int Id { get; set; }

    public string? Descripcion { get; set; }

    public int IdCategoria { get; set; }
    [JsonIgnore]
    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
