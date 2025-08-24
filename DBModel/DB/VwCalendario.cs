using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class VwCalendario
{
    public DateTime Fecha { get; set; }

    public int Anio { get; set; }

    public int Mes { get; set; }

    public int Dia { get; set; }

    public int DiaSemana { get; set; }

    public int SemanaAnio { get; set; }

    public int Feriado { get; set; }
}
