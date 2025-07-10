using DBModel.DB;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;
using UtilPaginados;

namespace IRepository
{
    public interface IProveedorRepository : ICRUDRepositorio<Proveedor>
    {
        Task<PaginacionResponse<ProveedorResponseTipo>> ListarProveedoresConTipoAsync(int pagina, int cantidad);
        Task<PaginacionResponse<Proveedor>> BuscarPaginadoAsync(string nombre, int pagina, int cantidad);
    }
}
