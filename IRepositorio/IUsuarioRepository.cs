using DBModel.DB;
using Models.RequestRequest;
using Models.ResponseResponse;
using UtilInterface;
using UtilPaginados;

namespace IRepository
{
    public interface IUsuarioRepository: ICRUDRepositorio<Usuario>
    {
        Usuario GetByUserName(string userName);
        Task<PaginacionResponse<Usuario>> GetUsuarios(int page, int pageSize);
        Task<UsuarioPersonaResponse> GetUsuarioPersona(int id);
        Task<bool> CrearUsuarioAsync(UsuarioRequest request);
    }
}