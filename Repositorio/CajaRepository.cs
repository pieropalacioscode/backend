using DBModel.DB;
using DocumentFormat.OpenXml.InkML;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilPaginados;

namespace Repository
{
    public class CajaRepository : GenericRepository<Caja>, ICajaRepository
    {
        public List<Caja> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }



        // Método para obtener todas las Cajas con fecha de hoy
        public async Task<Caja?> GetCajaDeHoy()
        {
            var today = DateTime.Today;

            // Devuelve la primera caja de hoy o null si no existe
            var cajaDeHoy = await dbSet
                .Where(c => c.Fecha.HasValue && c.Fecha.Value.Date == today)
                .FirstOrDefaultAsync();

            return cajaDeHoy;
        }


        // Método para buscar una caja por la fecha actual
        public Caja FindCajaByDate(DateTime date)
        {
            return dbSet.FirstOrDefault(c => c.Fecha.HasValue && c.Fecha.Value.Date == date.Date);
        }

        public async Task<PaginacionResponse<Caja>> GetCaja(int page, int pageSize)
        {
            var query = dbSet.AsQueryable();
            return await UtilPaginados.UtilPaginados.CrearPaginadoAsyncCaja(query, page, pageSize);
        }


    }
}
