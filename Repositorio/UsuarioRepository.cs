using DBModel.DB;
using DocumentFormat.OpenXml.Math;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Models.RequestRequest;
using Models.ResponseResponse;
using Repository.Generic;
using UtilPaginados;
using UtilSecurity.UtilSecurity;

namespace Repository
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {

        public List<Usuario> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public Usuario GetByUserName(string userName)
        {
            Usuario user = dbSet.Where(x => x.Username == userName).FirstOrDefault();
            return user;

        }

        public async Task<PaginacionResponse<Usuario>> GetUsuarios(int page, int pageSize)
        {
            var query = dbSet.AsQueryable();
            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, page, pageSize);
        }

        public async Task<UsuarioPersonaResponse> GetUsuarioPersona(int id)
        {
            var Usuario = await dbSet
                .Where(p => p.IdUsuario == id).
                Include(p=>p.IdPersonaNavigation).
                FirstOrDefaultAsync();
            if (Usuario == null) return null;
            return new UsuarioPersonaResponse
            {
                IdUsuario = Usuario.IdUsuario,
                Username = Usuario.Username,
                Cargo = Usuario.Cargo,
                Estado = Usuario.Estado,
                Nombre = Usuario.IdPersonaNavigation.Nombre,
                Apellido = Usuario.IdPersonaNavigation.ApellidoPaterno +" "+ Usuario.IdPersonaNavigation.ApellidoMaterno,
                TipoDocumento = Usuario.IdPersonaNavigation.TipoDocumento,
                NumeroDocumento = Usuario.IdPersonaNavigation.NumeroDocumento,
                Telefono = Usuario.IdPersonaNavigation.Telefono
            };
        }

        public async Task<bool> CrearUsuarioAsync(UsuarioRequest request)
        {
            // Verificar que la persona exista antes de crear el usuario
            var personaExiste = await db.Personas
                                       .AnyAsync(p => p.IdPersona == request.IdPersona);

            if (!personaExiste)
            {
                return false; // La persona no existe, retornamos false
            }

            // Encriptar la contraseña antes de guardarla
            string passwordEncriptada = UtilCripto.encriptar_AES(request.Password);

            // Crear el usuario con la referencia a la persona
            var usuario = new Usuario
            {
                Username = request.Username,
                Password = passwordEncriptada,
                Cargo = request.Cargo,
                Estado = request.Estado,
                IdPersona = request.IdPersona
            };

            // Agregar el usuario a la base de datos
            await db.Usuarios.AddAsync(usuario);
            await db.SaveChangesAsync();

            return true;
        }
    }
}