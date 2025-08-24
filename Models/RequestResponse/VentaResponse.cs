using DBModel.DB;
using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class VentaResponse
    {
        public int IdVentas { get; set; }

        public decimal? TotalPrecio { get; set; }

        public string? TipoComprobante { get; set; }

        public DateTime? FechaVenta { get; set; }

        public string? NroComprobante { get; set; }

        public int IdPersona { get; set; }

        public int IdUsuario { get; set; }
        public int IdCaja { get; set; }
        public string? TipoPago { get; set; }
        public decimal? Descuento { get; set; }
        public decimal? Vuelto { get; set; }
    }

    public class ResumenDashboardResponse
    {
        public int TotalComprobantes { get; set; }
        public decimal MontoTotalComprobantes { get; set; }

        public int TotalBoletas { get; set; }
        public decimal MontoBoletas { get; set; }

        public int TotalFacturas { get; set; }
        public decimal MontoFacturas { get; set; }

        public int TotalNotas { get; set; }
        public decimal MontoNotas { get; set; }
    }

    public class VentaResponsePago
    {
        public string? TipoPago { get; set; }
        public decimal? TotalPrecio { get; set; }
    }

    public class VentaHistoricaDto
    {
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
    }

    public class VentaPrediccionDto
    {
        public DateTime Fecha { get; set; }
        public float CantidadPredicha { get; set; }
        public DimFecha? DimFecha { get; set; }
    }

    public class VentasData
    {
        public DateTime Fecha { get; set; }
        public float Cantidad { get; set; }

        // Convertir a float
        public float EsFeriado { get; set; }
        public float EsPrevioFeriado { get; set; }
        public float EsDespuesFeriado { get; set; }
        public float EsFinDeSemana { get; set; }
        public float Mes { get; set; }
        public float Trimestre { get; set; }

        public string Estacion { get; set; }
    }


}
