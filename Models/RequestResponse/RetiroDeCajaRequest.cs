using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class RetiroDeCajaRequest
    {
        public int Id { get; set; }

        public string Descripcion { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public int CajaId { get; set; }

        public decimal MontoEfectivo { get; set; }

        public decimal MontoDigital { get; set; }
    }
}
