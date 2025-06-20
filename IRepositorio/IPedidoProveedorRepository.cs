using DBModel.DB;
using Models.RequestResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;

namespace IRepository
{
    public interface IPedidoProveedorRepository : ICRUDRepositorio<PedidoProveedor>
    {
        Task<List<PedidoProveedor>> getPorEstado(string estado);
        Task<PedidoDetalleResponse?> getPedidoconDetalle(int id);
    }
}
