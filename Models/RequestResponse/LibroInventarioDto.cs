using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class LibroInventarioDto
    {
        public int IdLibro { get; set; }
        public string? Titulo { get; set; }
        public string? Isbn { get; set; }
        public string? Tamanno { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public string? Impresion { get; set; }
        public string? Condicion { get; set; }
        public int Stock { get; set; } = 0;
        public decimal Precio { get; set; } = 0;
        public bool? Estado { get; set; }
        public decimal? PorcUtilidad { get; set; }
        public string TipoPapel { get; set; } = "Desconocido";

        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = "Desconocido";
    }
}
