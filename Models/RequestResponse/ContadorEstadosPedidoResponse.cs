using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class ContadorEstadosPedidoResponse
    {
        public int TotalIniciados { get; set; }
        public int TotalRecibidos { get; set; }
        public int TotalCancelados { get; set; }
    }
}
