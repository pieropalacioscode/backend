using DBModel.DB;
using IRepository;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class DetallePedidoProveedorRepository : GenericRepository<DetallePedidoProveedor>, IDetallePedidoProveedorRepository
    {
        public List<DetallePedidoProveedor> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }
    }
}
