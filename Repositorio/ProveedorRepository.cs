using DBModel.DB;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Models.RequestResponse;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilPaginados;

namespace Repository
{
    public class ProveedorRepository : GenericRepository<Proveedor>, IProveedorRepository
    {
        public List<Proveedor> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginacionResponse<ProveedorResponseTipo>> ListarProveedoresConTipoAsync(int pagina, int cantidad)
        {
            var query = dbSet
                .Include(p => p.IdTipoProveedorNavigation)
                .Select(p => new ProveedorResponseTipo
                {
                    IdProveedor = p.IdProveedor,
                    RazonSocial = p.RazonSocial,
                    Ruc = p.Ruc,
                    Direccion = p.Direccion,
                    Telefono=p.Telefono,
                    IdTipoProveedor = p.IdTipoProveedor,
                    NombreTipoProveedor = p.IdTipoProveedorNavigation.Descripcion
                });

            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }



        public async Task<PaginacionResponse<Proveedor>> BuscarPaginadoAsync(string nombre, int pagina, int cantidad)
        {
            var query = dbSet
                .Where(p => p.RazonSocial.Contains(nombre));
            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }
    }
}
