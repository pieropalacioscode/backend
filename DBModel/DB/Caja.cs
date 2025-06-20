﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DBModel.DB;

public partial class Caja
{
    public int IdCaja { get; set; }

    public decimal? SaldoInicial { get; set; }

    public decimal? SaldoFinal { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal? IngresosACaja { get; set; }

    public DateTime? FechaCierre { get; set; }

    public decimal? SaldoDigital { get; set; }

    public virtual ICollection<RetiroDeCaja> RetiroDeCajas { get; set; } = new List<RetiroDeCaja>();
    [JsonIgnore]
    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
