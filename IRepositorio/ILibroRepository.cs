using DBModel.DB;
using Models.RequestResponse;
using UtilInterface;
using UtilPaginados;

namespace IRepositorio
{
    public interface ILibroRepository : ICRUDRepositorio<Libro>
    {
        public Task<List<Libro>> GetByIds(List<int> ids);
        Task<Libro> GetLibroConPreciosYPublicoObjetivo(int libroId);
        Task<List<Precio>> GetPreciosByLibroId(int libroId);
        Task<Kardex> GetKardexByLibroId(int libroId);
        Task<(List<Libro>, int)> GetLibrosPaginados(int page, int pageSize);
        Task<List<Libro>> filtroComplete(string query);
        Task<PaginacionResponse<LibroDetalleResponse>> GetLibrosConDetallePaginadoAsync(int pagina, int cantidad);
        Task<PaginacionResponse<InventarioResponse>> GetInventarioPaginadoAsync(int pagina, int cantidad);
        Task<List<InventarioResponse>> BuscarEnInventario(string query);

    }
}