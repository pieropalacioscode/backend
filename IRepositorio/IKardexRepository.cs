using DBModel.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;

namespace IRepository
{
    public interface IKardexRepository : ICRUDRepositorio<Kardex>
    {
        void RegistrarEntradaKardex(int idLibro, int idSucursal, int cantidad, decimal precioUnitario);
        List<Kardex> GetLibrosConStock();
        Task<Kardex> GetByIdAsync(int id);
    }
}
