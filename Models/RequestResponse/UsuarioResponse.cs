using DBModel.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseResponse
{
    public class UsuarioResponse
    {
        public int IdUsuario { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Cargo { get; set; }

        public bool? Estado { get; set; }

        public int IdPersona { get; set; }
    }

    public class UsuarioPersonaResponse
    {
        public int IdUsuario { get; set; }

        public string? Username { get; set; }

        public string? Cargo { get; set; }

        public bool? Estado { get; set; }
        public string EstadoDescripcion
        {
            get
            {
                if (Estado == true) return "Activo";
                if (Estado == false) return "Inactivo";
                return "Sin definir";
            }
        }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Telefono { get;set; }

    }
}
