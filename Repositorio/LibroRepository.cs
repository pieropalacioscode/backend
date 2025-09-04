using DBModel.DB;
using DocumentFormat.OpenXml.InkML;
using IRepositorio;
using Microsoft.EntityFrameworkCore;
using Models.RequestResponse;
using Models.RequestResponse;
using Repository.Generic;
using UtilPaginados;





namespace Repository
{
    public class LibroRepository : GenericRepository<Libro>, ILibroRepository
    {


        public List<Libro> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LibroResponse.LibroConStockPrecioDto>> GetByIds(List<int> ids)
        {
            return await dbSet
                .Where(libro => ids.Contains(libro.IdLibro))
                .Select(libro => new LibroResponse.LibroConStockPrecioDto
                {
                    IdLibro = libro.IdLibro,
                    Titulo = libro.Titulo,
                    Isbn = libro.Isbn,
                    Tamanno = libro.Tamanno,
                    Descripcion = libro.Descripcion,
                    Condicion = libro.Condicion,
                    Impresion = libro.Impresion,
                    TipoTapa = libro.TipoTapa,
                    Estado = libro.Estado,
                    IdSubcategoria = libro.IdSubcategoria,
                    IdTipoPapel = libro.IdTipoPapel,
                    IdProveedor = libro.IdProveedor,
                    Imagen = libro.Imagen,
                    PrecioVenta = libro.Precios
                        .Select(p => p.PrecioVenta ?? 0)
                        .FirstOrDefault(), // solo un precio, si hay varios toma el primero
                    Stock = libro.Kardex != null ? libro.Kardex.Stock ?? 0 : 0
                })
                .ToListAsync();
        }



        public async Task<Libro> GetLibroConPreciosYPublicoObjetivo(int libroId)
        {
            return await dbSet.Where(l => l.IdLibro == libroId)
                .Include(l => l.Precios)
                .ThenInclude(p => p.IdPublicoObjetivoNavigation)
                .FirstOrDefaultAsync();
        }
        public async Task<Libro> GetByIdAsync(object id)
        {
            return await dbSet.Where(libro => libro.IdLibro == (int)id).FirstOrDefaultAsync();
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
            if (string.IsNullOrWhiteSpace(query))
                return new List<Libro>();

            var palabras = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var librosQuery = dbSet
                .Include(l => l.LibroAutors)
                    .ThenInclude(la => la.IdAutorNavigation)
                .AsQueryable();

            // Agregamos condiciones dinámicamente
            foreach (var palabra in palabras)
            {
                librosQuery = librosQuery.Where(libro =>
                    libro.Titulo.ToLower().Contains(palabra) ||
                    libro.Isbn.ToLower().Contains(palabra) ||
                    libro.LibroAutors.Any(la =>
                        la.IdAutorNavigation.Nombre.ToLower().Contains(palabra) ||
                        la.IdAutorNavigation.Apellido.ToLower().Contains(palabra))
                );
            }

            return await librosQuery
                .OrderBy(l => l.Titulo)
                .Take(20) // puedes ajustar el límite
                .ToListAsync();
        }

        public async Task<List<InventarioResponse>> BuscarEnInventario(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<InventarioResponse>();

            var palabras = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var librosQuery = dbSet
                .Include(l => l.Kardex)
                .Include(l => l.IdProveedorNavigation)
                .Include(l => l.IdSubcategoriaNavigation)
                .Include(l => l.IdTipoPapelNavigation)
                .Include(l => l.LibroAutors)
                    .ThenInclude(la => la.IdAutorNavigation)
                .Where(l => l.Estado == true)
                .AsQueryable();

            // Tokenización para buscar por cada palabra
            foreach (var palabra in palabras)
            {
                librosQuery = librosQuery.Where(l =>
                    l.Titulo!.ToLower().Contains(palabra) ||
                    l.Isbn!.ToLower().Contains(palabra) ||
                    l.LibroAutors.Any(la =>
                        la.IdAutorNavigation.Nombre!.ToLower().Contains(palabra) ||
                        la.IdAutorNavigation.Apellido!.ToLower().Contains(palabra))
                );
            }

            // Proyección al modelo InventarioResponse
            var resultado = await librosQuery
                .OrderBy(l => l.Titulo)
                .Take(20)
                .Select(l => new InventarioResponse
                {
                    IdLibro = l.IdLibro,
                    Titulo = l.Titulo ?? "",
                    Isbn = l.Isbn ?? "",
                    Stock = l.Kardex != null ? l.Kardex.Stock ?? 0 : 0,
                    UltPrecioCosto = l.Kardex != null ? l.Kardex.UltPrecioCosto ?? 0 : 0,
                    Proveedor = l.IdProveedorNavigation.RazonSocial ?? "",
                    Subcategoria = l.IdSubcategoriaNavigation.Descripcion ?? "",
                    TipoPapel = l.IdTipoPapelNavigation.Descripcion ?? "",
                    Autores = l.LibroAutors
                        .Select(la => $"{la.IdAutorNavigation.Nombre} {la.IdAutorNavigation.Apellido}".Trim())
                        .ToList()
                })
                .ToListAsync();

            return resultado;
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


        public async Task<PaginacionResponse<InventarioResponse>> GetInventarioPaginadoAsync(int pagina, int cantidad)
        {
            var query = dbSet
                .Include(l => l.Kardex)
                .Include(l => l.IdProveedorNavigation)
                .Include(l => l.IdSubcategoriaNavigation)
                .Include(l => l.IdTipoPapelNavigation)
                .Include(l => l.LibroAutors)
                    .ThenInclude(la => la.IdAutorNavigation)
                .Where(l => l.Estado == true)
                .Select(l => new InventarioResponse
                {
                    IdLibro = l.IdLibro,
                    Titulo = l.Titulo,
                    Isbn = l.Isbn,
                    Stock = l.Kardex != null ? l.Kardex.Stock ?? 0 : 0,
                    UltPrecioCosto = l.Kardex != null ? l.Kardex.UltPrecioCosto ?? 0 : 0,
                    Proveedor = l.IdProveedorNavigation.RazonSocial ?? "",
                    Subcategoria = l.IdSubcategoriaNavigation.Descripcion ?? "",
                    TipoPapel = l.IdTipoPapelNavigation.Descripcion ?? "",
                    Autores = l.LibroAutors
                        .Select(la => $"{la.IdAutorNavigation.Nombre} {la.IdAutorNavigation.Apellido}".Trim())
                        .ToList()
                });

            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }


        public async Task<(List<Libro>, int)> FiltrarLibrosAsync(
            bool? estado,
            string titulo,
            int? idCategoria,
            int? idSubcategoria,
            int page,
            int pageSize)
        {
            var query = dbSet
                .Include(l => l.IdSubcategoriaNavigation)
                    .ThenInclude(s => s.IdCategoriaNavigation) // asegurar relación
                .AsQueryable();

            // Filtrar por estado
            if (estado.HasValue)
            {
                query = query.Where(l => l.Estado == estado.Value);
            }

            // Filtrar por título
            if (!string.IsNullOrWhiteSpace(titulo))
            {
                query = query.Where(l => EF.Functions.Like(l.Titulo, $"%{titulo}%"));
            }

            // Filtrar por subcategoría
            if (idSubcategoria.HasValue)
            {
                query = query.Where(l => l.IdSubcategoria == idSubcategoria.Value);
            }

            // Filtrar por categoría
            if (idCategoria.HasValue)
            {
                query = query.Where(l => l.IdSubcategoriaNavigation.IdCategoria == idCategoria.Value);
            }

            // Total para paginación
            int totalItems = await query.CountAsync();

            // Paginación
            var libros = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (libros, totalItems);
        }


        public async Task<bool> CambiarEstadoLibro(int libroId)
        {
            var libro = await dbSet.FindAsync(libroId);
            if (libro == null)
            {
                return false; // Retornar false si el libro no existe
            }

            libro.Estado = false; // Alternar estado
            await db.SaveChangesAsync();

            return true; // Retornar true si se actualizó correctamente
        }

        public async Task<PaginacionResponse<Libro>> ListarPorProveedorPaginadoAsync(int idProveedor, int pagina, int cantidad)
        {
            var query = dbSet
                .Where(l => l.IdProveedor == idProveedor && l.Estado == true)

                .AsQueryable();

            return await UtilPaginados.UtilPaginados.CrearPaginadoAsync(query, pagina, cantidad);
        }


        public async Task<(List<LibroInventarioDto> Libros, int TotalItems)> FiltrarLibrosProveedorAsync(int? idProveedor,string? titulo = null,int page = 1,int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = dbSet
                .Include(l => l.IdProveedorNavigation)
                .Include(l => l.Kardex)
                .Include(l => l.Precios)
                .Include(l => l.IdTipoPapelNavigation)
                .AsQueryable();

            // Filtrar por proveedor
            if (idProveedor is > 0)
                query = query.Where(l => l.IdProveedor == idProveedor.Value);

            if (!string.IsNullOrWhiteSpace(titulo))
            {
                var palabras = titulo.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var palabra in palabras)
                {
                    string palabraLower = palabra.ToLower();
                    query = query.Where(l => l.Titulo != null && l.Titulo.ToLower().Contains(palabraLower));
                }
            }


            var totalItems = await query.CountAsync();

            var librosRaw = await query
                .OrderBy(l => l.IdLibro)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(l => l.Kardex)
                .Include(l => l.Precios)
                .Include(l => l.IdTipoPapelNavigation)
                .Include(l => l.IdProveedorNavigation)
                .ToListAsync(); 

            var libros = librosRaw
                .Select(l => new LibroInventarioDto
                {
                    IdLibro = l.IdLibro,
                    Titulo = l.Titulo,
                    Isbn = l.Isbn,
                    Tamanno = l.Tamanno,
                    Descripcion = l.Descripcion,
                    Imagen = l.Imagen,
                    Impresion = l.Impresion,
                    Condicion = l.Condicion,
                    Estado = l.Estado ?? false,
                    Stock = l.Kardex?.Stock ?? 0,
                    PorcUtilidad = l.Precios.OrderByDescending(p=>p.IdPrecios).FirstOrDefault()?.PorcUtilidad ?? 0,
                    Precio = l.Precios.OrderByDescending(p => p.IdPrecios).FirstOrDefault()?.PrecioVenta ?? 0,
                    TipoPapel = l.IdTipoPapelNavigation?.Descripcion ?? "Desconocido",
                    IdProveedor = l.IdProveedor,
                    NombreProveedor = l.IdProveedorNavigation?.RazonSocial ?? "Desconocido"
                })
                .ToList();


            return (libros, totalItems);
        }




    }
}  

