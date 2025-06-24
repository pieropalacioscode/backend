using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class InventarioResponse
    {
        public int IdLibro { get; set; }
        public string? Titulo { get; set; }
        public string? Isbn { get; set; }
        public int Stock { get; set; }
        public decimal UltPrecioCosto { get; set; }
        public string Proveedor { get; set; } = null!;
        public string Subcategoria { get; set; } = null!;
        public string TipoPapel { get; set; } = null!;
        public List<string> Autores { get; set; } = new();
    }
}
