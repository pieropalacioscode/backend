using DBModel.DB;
using DocumentFormat.OpenXml.InkML;
using IRepositorio;
using Microsoft.EntityFrameworkCore;
using Models.RequestResponse;
using Repository.Generic;
using UtilPaginados;
using Models.RequestResponse;

namespace Repository
{
    public class LibroRepository : GenericRepository<Libro>, ILibroRepository
    {


        public List<Libro> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Libro>> GetByIds(List<int> ids)
        {
            return await dbSet.Where(libro => ids.Contains(libro.IdLibro)).ToListAsync();
        }

        public async Task<Libro> GetLibroConPreciosYPublicoObjetivo(int libroId)
        {
            return await dbSet.Where(l => l.IdLibro == libroId)
                .Include(l => l.Precios)
                .ThenInclude(p => p.IdPublicoObjetivoNavigation)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Precio>> GetPreciosByLibroId(int libroId)

        {
            var libro = await dbSet
                .Include(l => l.Precios)
                .FirstOrDefaultAsync(l => l.IdLibro == libroId);

            // Verificar si el libro existe y tiene precios asociados
            if (libro != null && libro.Precios != null)
            {
                return libro.Precios.ToList();
            }

            // Si no hay precios asociados, devolver una lista vacía
            return new List<Precio>();
        }

        public async Task<Kardex> GetKardexByLibroId(int libroId)
        {
            var libro = await dbSet
                .Include(l => l.Kardex)
                .FirstOrDefaultAsync(l => l.IdLibro == libroId);

            return libro?.Kardex;
        }

        public async Task<(List<Libro>, int)> GetLibrosPaginados(int page, int pageSize)
        {
            var query = dbSet.AsQueryable();
            int totalItems = await query.CountAsync();
            var libros = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (libros, totalItems);
        }


        public async Task<List<Libro>> filtroComplete(string query)
        {
            return await dbSet
                .Where(libro => EF.Functions.Like(libro.Titulo, $"%{query}%"))
                .ToListAsync();
        }


        public async Task<PaginacionResponse<LibroDetalleResponse>> GetLibrosConDetallePaginadoAsync(int pagina, int cantidad)
        {
            var query = dbSet
                .Include(l => l.IdProveedorNavigation)
                .Include(l => l.IdSubcategoriaNavigation)
                .Include(l => l.IdTipoPapelNavigation)
                .Select(libro => new LibroDetalleResponse
                {
                    IdLibro = libro.IdLibro,
                    Titulo = libro.Titulo,
                    Isbn = libro.Isbn,
                    Tamanno = libro.Tamanno,
                    Descripcion = libro.Descripcion,
                    Condicion = libro.Condicion,
                    Impresion = libro.Impresion,
                    TipoTapa = libro.TipoTapa,
                    Imagen = libro.Imagen,
                    Estado = libro.Estado ?? false,

                    Proveedor = new ProveedorResponse
                    {
                        IdProveedor = libro.IdProveedorNavigation.IdProveedor,
                        RazonSocial = libro.IdProveedorNavigation.RazonSocial,
                        Ruc = libro.IdProveedorNavigation.Ruc,
                        Direccion = libro.IdProveedorNavigation.Direccion
                        
                    },
                    Subcategoria = new SubcategoriaResponse
                    {
                        Id = libro.IdSubcategoriaNavigation.Id,
                        Descripcion = libro.IdSubcategoriaNavigation.Descripcion,
                        IdCategoria = libro.IdSubcategoriaNavigation.IdCategoria
                    },
                    TipoPapel = new TipoPapelResponse
                    {
                        IdTipoPapel = libro.IdTipoPapelNavigation.IdTipoPapel,
                        Descripcion = libro.IdTipoPapelNavigation.Descripcion
                    }
                });


            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }

    }
}  

