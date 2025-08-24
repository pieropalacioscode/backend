using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class VwVentasDiaria
{
    public DateTime? Fecha { get; set; }

    public int IdSucursal { get; set; }

    public int? TotalCantidad { get; set; }

    public decimal? TotalImporte { get; set; }

    public int? TotalTransacciones { get; set; }
}
