using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class Notificacion
{
    public int Id { get; set; }

    public string Mensaje { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = null!;

    public int? IdLibro { get; set; }

    public bool Leido { get; set; }

    public virtual Libro? IdLibroNavigation { get; set; }
}
