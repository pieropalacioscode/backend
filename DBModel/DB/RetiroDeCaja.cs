using System;
using System.Collections.Generic;

namespace DBModel.DB;

public partial class RetiroDeCaja
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public int CajaId { get; set; }

    public decimal MontoEfectivo { get; set; }

    public decimal MontoDigital { get; set; }

    public virtual Caja Caja { get; set; } = null!;
}
