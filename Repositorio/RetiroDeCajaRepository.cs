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
    public class RetiroDeCajaRepository : GenericRepository<RetiroDeCaja>, IRetiroDeCajaRepository
    {
        public List<RetiroDeCaja> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> GetTotalRetirosHoy()
        {
            var hoy = DateTime.Today;

            var total = await dbSet
                .Where(r => r.Fecha.Date == hoy)
                .SumAsync(r => r.MontoEfectivo + r.MontoDigital);

            return total;
        }

        public async Task<List<RetiroDeCaja>> GetRetirosPorCajaAsync(int idCaja)
        {
            return await dbSet
                .Where(r => r.CajaId == idCaja)
                .ToListAsync();
        }


    }
}
