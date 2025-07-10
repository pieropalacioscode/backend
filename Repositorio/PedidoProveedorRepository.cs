using DBModel.DB;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
                .Include(p => p.IdPersonaNavigation) // 👈 incluir al cliente
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
                Imagen = pedido.Imagen,

                // NUEVO
                IdPersona = pedido.IdPersona,
                NombreCliente = pedido.IdPersonaNavigation != null
                    ? $"{pedido.IdPersonaNavigation.Nombre} {pedido.IdPersonaNavigation.ApellidoPaterno}"
                    : null,

                Detalles = pedido.DetallePedidoProveedors.Select(d => new LibroPedidoDetalleDto
                {
                    Id = d.Id,
                    idLibro = d.IdLibro,
                    Titulo = d.IdLibroNavigation.Titulo,
                    Isbn = d.IdLibroNavigation.Isbn,
                    Imagen = d.IdLibroNavigation.Imagen,
                    CantidadPedida = d.CantidadPedida,
                    CantidadRecibida = d.CantidadRecibida,
                    PrecioUnitario = d.PrecioUnitario,
                    Lote = d.Lote
                }).ToList()
            };
        }



        public async Task<PaginacionResponse<PedidoDetalleResponse>> GetPedidosPorFechaPaginado(DateTime fecha, int pagina, int cantidad)
        {
            var query = dbSet
                .Include(p => p.DetallePedidoProveedors)
                    .ThenInclude(d => d.IdLibroNavigation)
                .Include(p => p.IdProveedorNavigation)
                .Where(p => p.Fecha.Date == fecha.Date)
                .OrderByDescending(p => p.Fecha)
                .Select(pedido => new PedidoDetalleResponse
                {
                    Id = pedido.Id,
                    Fecha = pedido.Fecha,
                    Estado = pedido.Estado,
                    DescripcionPedido = pedido.DescripcionPedido,
                    DescripcionRecepcion = pedido.DescripcionRecepcion,
                    Proveedor = pedido.IdProveedorNavigation.RazonSocial,
                    idProveedor = pedido.IdProveedor,
                    Imagen = pedido.Imagen,
                    Detalles = pedido.DetallePedidoProveedors.Select(d => new LibroPedidoDetalleDto
                    {
                        Id = d.Id,
                        idLibro = d.IdLibro,
                        Titulo = d.IdLibroNavigation.Titulo,
                        Isbn = d.IdLibroNavigation.Isbn,
                        Imagen = d.IdLibroNavigation.Imagen,
                        CantidadPedida = d.CantidadPedida,
                        CantidadRecibida = d.CantidadRecibida,
                        PrecioUnitario = d.PrecioUnitario,
                        Lote = d.Lote
                    }).ToList()
                });

            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }



        public async Task<PaginacionResponse<PedidoDetalleResponse>> GetPedidosConDetallesPaginado(string estado, int pagina, int cantidad)
        {
            var query = dbSet
                .Where(p => p.Estado.ToLower() == estado.ToLower())
                .OrderByDescending(p => p.Fecha)
                .Include(p => p.DetallePedidoProveedors)
                    .ThenInclude(d => d.IdLibroNavigation)
                .Include(p => p.IdProveedorNavigation)
                .Select(pedido => new PedidoDetalleResponse
                {
                    Id = pedido.Id,
                    Fecha = pedido.Fecha,
                    Estado = pedido.Estado,
                    DescripcionPedido = pedido.DescripcionPedido,
                    DescripcionRecepcion = pedido.DescripcionRecepcion,
                    Proveedor = pedido.IdProveedorNavigation.RazonSocial,
                    idProveedor = pedido.IdProveedor,
                    Imagen = pedido.Imagen,
                    Detalles = pedido.DetallePedidoProveedors.Select(d => new LibroPedidoDetalleDto
                    {
                        Id = d.Id,
                        idLibro = d.IdLibro,
                        Titulo = d.IdLibroNavigation.Titulo,
                        Isbn = d.IdLibroNavigation.Isbn,
                        Imagen = d.IdLibroNavigation.Imagen,
                        CantidadPedida = d.CantidadPedida,
                        CantidadRecibida = d.CantidadRecibida,
                        PrecioUnitario = d.PrecioUnitario,
                        Lote = d.Lote
                    }).ToList()
                });

            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }

        public async Task<ContadorEstadosPedidoResponse> getcanEstado()
        {
            var totalIniciados = await dbSet.CountAsync(p => p.Estado.ToLower() == "Iniciado");
            var totalRecibidos = await dbSet.CountAsync(p => p.Estado.ToLower() == "Recibido");
            var totalCancelados = await dbSet.CountAsync(p => p.Estado.ToLower() == "Cancelado");

            return new ContadorEstadosPedidoResponse
            {
                TotalIniciados = totalIniciados,
                TotalRecibidos = totalRecibidos,
                TotalCancelados = totalCancelados
            };
        }

        public async Task<List<PedidoDetalleResponse>> GetPedidosConDetallesPorFechaYProveedor(DateTime fecha, int idProveedor)
        {
            var pedidos = await dbSet
                .Include(p => p.DetallePedidoProveedors)
                    .ThenInclude(d => d.IdLibroNavigation)
                .Include(p => p.IdProveedorNavigation)
                .Include(p => p.IdPersonaNavigation)
                .Where(p => p.Fecha.Date == fecha.Date && p.IdProveedor == idProveedor)
                .ToListAsync();

            return pedidos.Select(p => new PedidoDetalleResponse
            {
                Id = p.Id,
                Fecha = p.Fecha,
                Estado = p.Estado,
                DescripcionPedido = p.DescripcionPedido,
                DescripcionRecepcion = p.DescripcionRecepcion,
                Proveedor = p.IdProveedorNavigation.RazonSocial,
                idProveedor = p.IdProveedor,
                NombreCliente = p.IdPersonaNavigation != null ? $"{p.IdPersonaNavigation.Nombre} {p.IdPersonaNavigation.ApellidoPaterno}" : "No asignado",
                Detalles = p.DetallePedidoProveedors.Select(d => new LibroPedidoDetalleDto
                {
                    Id = d.Id,
                    idLibro = d.IdLibro,
                    Titulo = d.IdLibroNavigation.Titulo,
                    Isbn = d.IdLibroNavigation.Isbn,
                    Imagen = d.IdLibroNavigation.Imagen,
                    CantidadPedida = d.CantidadPedida,
                    CantidadRecibida = d.CantidadRecibida,
                    PrecioUnitario = d.PrecioUnitario,
                    Lote = d.Lote
                }).ToList()
            }).ToList();
        }



    }
}
