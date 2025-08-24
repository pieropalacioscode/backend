using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class Feriado
{
    public int IdFeriado { get; set; }

    public DateTime Fecha { get; set; }

    public string Descripcion { get; set; } = null!;
}
