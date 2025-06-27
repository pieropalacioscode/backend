using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class NotificacionResponse
    {
        public int Id { get; set; }

        public string Mensaje { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public string Tipo { get; set; } = null!;

        public int? IdLibro { get; set; }

        public bool Leido { get; set; }
    }
}
