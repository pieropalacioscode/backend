using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class TasaRotacionResponse
    {
        public int IdLibro { get; set; }
        public string Titulo { get; set; }
        public int StockInicial { get; set; }
        public int TotalVendido { get; set; }
        public double TasaRotacion { get; set; }
    }
}
