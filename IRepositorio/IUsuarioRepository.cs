using DBModel.DB;
using Models.ResponseResponse;
using UtilInterface;
using UtilPaginados;

namespace IRepository
{
    public interface IUsuarioRepository: ICRUDRepositorio<Usuario>
    {
        Usuario GetByUserName(string userName);
        Task<PaginacionResponse<Usuario>> GetUsuarios(int page, int pageSize);
    }
}