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

namespace Repository
{
    public class PedidoProveedorRepository : GenericRepository<PedidoProveedor>, IPedidoProveedorRepository
    {
        public List<PedidoProveedor> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PedidoProveedor>> getPorEstado(string estado)
        {
            var pedidos = await dbSet
                .Where(p => p.Estado.ToLower() == estado.ToLower())
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
            return pedidos;
        }
        public async Task<PedidoDetalleResponse?> getPedidoconDetalle(int id)
        {
            var pedido = await dbSet
        .Where(p => p.Id == id)
        .Include(p => p.DetallePedidoProveedors)
            .ThenInclude(d => d.IdLibroNavigation)
        .Include(p => p.IdProveedorNavigation)
        .FirstOrDefaultAsync();

            if (pedido == null) return null;

            return new PedidoDetalleResponse
            {
                Id = pedido.Id,
                Fecha = pedido.Fecha,
                Estado = pedido.Estado,
                DescripcionPedido = pedido.DescripcionPedido,
                DescripcionRecepcion = pedido.DescripcionRecepcion,
                Proveedor = pedido.IdProveedorNavigation.RazonSocial,
                idProveedor = pedido.IdProveedor,
                Detalles = pedido.DetallePedidoProveedors.Select(d => new LibroPedidoDetalleDto
                {
                    Id = d.Id,
                    idLibro =d.IdLibro,
                    Titulo = d.IdLibroNavigation.Titulo,
                    Isbn = d.IdLibroNavigation.Isbn,
                    Imagen = d.IdLibroNavigation.Imagen,
                    CantidadPedida = d.CantidadPedida,
                    CantidadRecibida = d.CantidadRecibida,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };
        }

    }
}
