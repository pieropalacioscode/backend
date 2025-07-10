using DBModel.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;
using UtilPaginados;

namespace IRepository
{
    public interface ICajaRepository : ICRUDRepositorio<Caja>  
    {
        Caja FindCajaByDate(DateTime date);
        Task<PaginacionResponse<Caja>> GetCaja(int page, int pageSize);
    }
}
