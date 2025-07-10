using DBModel.DB;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AutorRepository : GenericRepository<Autor>, IAutorRepository
    {
        public List<Autor> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<Autor> GetByIdAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<Autor> GetByName(string nombre)
        {
            return await dbSet.FirstOrDefaultAsync(autor => autor.Nombre.ToLower() == nombre.ToLower());
        }
    }
}
