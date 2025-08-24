using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class VwDataEntrenamiento
{
    public DateTime? Fecha { get; set; }

    public int IdSucursal { get; set; }

    public int? TotalCantidad { get; set; }

    public decimal? TotalImporte { get; set; }

    public int? TotalTransacciones { get; set; }

    public int Anio { get; set; }

    public int Mes { get; set; }

    public int DiaSemana { get; set; }

    public int Feriado { get; set; }
}
