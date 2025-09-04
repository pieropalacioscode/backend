using Microsoft.EntityFrameworkCore;

namespace UtilPaginados

{
    public static class UtilPaginados
    {
        public static async Task<PaginacionResponse<T>> CrearPaginadoAsync<T>(
                IQueryable<T> query, int pagina, int cantidad)
        {
            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)cantidad);

            var items = await query
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();

            return new PaginacionResponse<T>
            {
                Items = items,
                Total = total,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };
        }

        public static async Task<PaginacionResponse<T>> CrearPaginadoAsyncCaja<T>(IQueryable<T> query, int pagina, int cantidad) where T : class
        {
            var total = await query.CountAsync();
            var totalPaginas = (int)Math.Ceiling(total / (double)cantidad);

            // Ordenar descendente según la propiedad que necesites (Id o Fecha)
            // Si Caja tiene Id autoincremental:
            //query = query.OrderByDescending(e => EF.Property<int>(e, "Id"));

            // O si prefieres por Fecha:
            query = query.OrderByDescending(e => EF.Property<DateTime>(e, "Fecha"));

            var items = await query
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();

            return new PaginacionResponse<T>
            {
                Items = items,
                Total = total,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };
        }


    }
}
