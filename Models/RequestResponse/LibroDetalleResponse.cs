using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestResponse
{
    public class LibroDetalleResponse
    {
        public int IdLibro { get; set; }
        public string? Titulo { get; set; }
        public string? Isbn { get; set; }
        public string? Tamanno { get; set; }
        public string? Descripcion { get; set; }
        public string? Condicion { get; set; }
        public string? Impresion { get; set; }
        public string? TipoTapa { get; set; }
        public string? Imagen { get; set; }

        // Estado como texto
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
        public bool Estado { get; set; }

        // Relaciones
        public ProveedorResponse Proveedor { get; set; } = null!;
        public SubcategoriaResponse Subcategoria { get; set; } = null!;
        public TipoPapelResponse TipoPapel { get; set; } = null!;
    }

}
