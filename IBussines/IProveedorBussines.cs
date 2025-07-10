using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;
using UtilPaginados;

namespace IBussines
{
    public interface IProveedorBussines : ICRUDBussnies<ProveedorRequest, ProveedorResponse>
    {
        Task<PaginacionResponse<ProveedorResponseTipo>> ListarPaginadoAsync(int pagina, int cantidad);
        Task<PaginacionResponse<ProveedorResponse>> BuscarPaginadoAsync(string nombre, int pagina, int cantidad);
    }
}
