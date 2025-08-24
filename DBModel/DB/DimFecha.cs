using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class DimFecha
{
    public int IdFecha { get; set; }

    public DateTime Fecha { get; set; }

    public int? Anio { get; set; }

    public int? Mes { get; set; }

    public string? NombreMes { get; set; }

    public int? Dia { get; set; }

    public string? NombreDia { get; set; }

    public bool? EsFeriado { get; set; }

    public bool? EsPrevioFeriado { get; set; }

    public bool? EsDespuesFeriado { get; set; }

    public bool? EsFinDeSemana { get; set; }

    public string? TipoDia { get; set; }

    public int? Trimestre { get; set; }

    public string? Estacion { get; set; }
}
