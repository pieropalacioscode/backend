using DBModel.DB;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Models.ResponseResponse;
using Repository.Generic;
using UtilPaginados;

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


    }
}