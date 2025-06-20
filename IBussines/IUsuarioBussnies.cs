using DBModel.DB;
using Models.RequestRequest;
using Models.RequestResponse;
using Models.ResponseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;
using UtilPaginados;

namespace IBussnies
{
    public interface IUsuarioBussnies : ICRUDBussnies<UsuarioRequest, UsuarioResponse>
    {
        UsuarioResponse GetByUserName(string userName);
        Task<PaginacionResponse<Usuario>> GetUsuarios(int page, int pageSize);
    }
}
