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
    public class DimFechaRepository : GenericRepository<DimFecha>, IDimFechaRepository
    {
        public List<DimFecha> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }
        public async Task<List<DimFecha>> GetAllAsync()
        {
            return await dbSet
                .OrderBy(f => f.Fecha) // Ordenamos por la fecha
                .ToListAsync();
        }

    }
}
