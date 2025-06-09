using DBModel.DB;
using DocumentFormat.OpenXml.InkML;
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
    public class VentaRepository: GenericRepository<Venta>,IVentaRepository
    {

        public List<Venta> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DetalleVenta>> GetDetallesByVentaId(int idVenta)
        {
            var ventaConDetalles = await dbSet
                .Include(v => v.DetalleVenta) // Usar el nombre correcto de la propiedad
                .FirstOrDefaultAsync(v => v.IdVentas == idVenta); // Asegúrate de usar el nombre correcto del ID

            return ventaConDetalles?.DetalleVenta.ToList() ?? new List<DetalleVenta>();
        }

        public async Task<(Venta venta, List<DetalleVenta> detalles)> GetVentaConDetalles(int idVenta)
        {
            var ventaConDetalles = await dbSet
                .Include(v => v.DetalleVenta) // Cargar los detalles de venta
                .FirstOrDefaultAsync(v => v.IdVentas == idVenta);

            // Si no se encuentra la venta, devolver una venta nula y una lista vacía de detalles.
            if (ventaConDetalles == null) return (null, new List<DetalleVenta>());

            // Devolver tanto la venta como los detalles de venta.
            return (ventaConDetalles, ventaConDetalles.DetalleVenta.ToList());
        }

        public async Task<Persona> GetPersonaByVentaId(int idVenta)
        {
            // Intenta obtener la venta incluyendo los detalles de la persona asociada usando la propiedad de navegación.
            var venta = await dbSet
                .Include(v => v.IdPersonaNavigation)  // Usar la propiedad de navegación para incluir la entidad Persona.
                .FirstOrDefaultAsync(v => v.IdVentas == idVenta);

            // Devuelve la persona asociada, o null si no se encuentra la venta.
            return venta?.IdPersonaNavigation;
        }

        public async Task<string> ObtenerUltimoNumeroComprobantePorTipo(string prefijo)
        {
            var ultimoComprobante = await db.Ventas
                .Where(v => v.NroComprobante.StartsWith(prefijo)) // Filtra por prefijo
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => v.NroComprobante)
                .FirstOrDefaultAsync();

            return ultimoComprobante;
        }



        public async Task<IEnumerable<Venta>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await dbSet
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta < fechaFin.AddDays(1))
            .ToListAsync();

        }


        public async Task<(List<Venta>, int)> GetVentaPaginados(int page, int pageSize)
        {
            var query = dbSet.AsQueryable();
            int totalItems = await query.CountAsync();
            var venta = await query.Skip((page-1) * pageSize).Take(pageSize).ToListAsync();
            return (venta, totalItems);
        }


        public async Task<(int totalComprobantes, decimal montoTotalComprobantes)> ObtenerResumenDashboardAsync()
        {
            var ventas = await dbSet
                .Where(v => v.TipoComprobante != null &&
                            (v.TipoComprobante.ToLower() == "boleta" || v.TipoComprobante.ToLower() == "factura"))
                .ToListAsync();

            var totalComprobantes = ventas.Count;
            var montoTotalComprobantes = ventas.Sum(v => v.TotalPrecio ?? 0);

            return (totalComprobantes, montoTotalComprobantes);
        }


        public async Task<List<IngresoMensualResponse>> ObtenerIngresosPorMes(int mes)
        {
            var anioActual = DateTime.Now.Year;
            var fechaInicio = new DateTime(anioActual, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1); // último día del mes

            const string query = @"
        SELECT 
            FORMAT(v.Fecha_Venta, 'yyyy-MM') AS MesAño,
            SUM(d.Importe) AS TotalIngresos
        FROM Ventas v
        INNER JOIN Detalle_Ventas d ON v.Id_Ventas = d.id_Ventas
        WHERE v.Fecha_Venta BETWEEN @fechaInicio AND @fechaFin
        GROUP BY FORMAT(v.Fecha_Venta, 'yyyy-MM')
        ORDER BY MesAño";

            var ingresosMensuales = new List<IngresoMensualResponse>();

            await using var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = query;

            var paramInicio = command.CreateParameter();
            paramInicio.ParameterName = "@fechaInicio";
            paramInicio.Value = fechaInicio;
            command.Parameters.Add(paramInicio);

            var paramFin = command.CreateParameter();
            paramFin.ParameterName = "@fechaFin";
            paramFin.Value = fechaFin;
            command.Parameters.Add(paramFin);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ingresosMensuales.Add(new IngresoMensualResponse
                {
                    MesAño = reader.GetString(0),
                    TotalIngresos = reader.GetDecimal(1)
                });
            }

            return ingresosMensuales;
        }


        public async Task<List<TasaRotacionResponse>> ObtenerTasaRotacionInventario(string filtro, int offset, int limit)
        {
            string orderByClause = filtro.ToLower() switch
            {
                "tasa" => "TasaRotacionAprox DESC",
                "total" => "TotalVendidos DESC",
                _ => "TotalVendidos DESC"
            };

            string query = $@"
SELECT 
    l.IdLibro,
    l.Titulo,
    ISNULL(SUM(d.Cantidad), 0) AS TotalVendidos,
    k.Stock AS StockActual,
    CASE 
        WHEN (k.Stock + ISNULL(SUM(d.Cantidad), 0)) = 0 THEN 0
        ELSE CAST(ISNULL(SUM(d.Cantidad), 0) AS FLOAT) / 
             ((k.Stock + ISNULL(SUM(d.Cantidad), 0)) / 2.0)
    END AS TasaRotacionAprox
FROM Libro l
JOIN Kardex k ON l.IdLibro = k.IdLibro
LEFT JOIN Detalle_Ventas d ON l.IdLibro = d.IdLibro
GROUP BY l.IdLibro, l.Titulo, k.Stock
ORDER BY " + orderByClause + @"
OFFSET @offset ROWS
FETCH NEXT @limit ROWS ONLY;";

            using var connection = db.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = query;

            var paramOffset = command.CreateParameter();
            paramOffset.ParameterName = "@offset";
            paramOffset.Value = offset;
            command.Parameters.Add(paramOffset);

            var paramLimit = command.CreateParameter();
            paramLimit.ParameterName = "@limit";
            paramLimit.Value = limit;
            command.Parameters.Add(paramLimit);

            using var reader = await command.ExecuteReaderAsync();
            var resultado = new List<TasaRotacionResponse>();

            while (await reader.ReadAsync())
            {
                resultado.Add(new TasaRotacionResponse
                {
                    IdLibro = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    StockInicial = reader.GetInt32(3),
                    TotalVendido = reader.GetInt32(2),
                    TasaRotacion = Math.Round(reader.GetDouble(4), 2)
                });
            }

            return resultado;
        }



    }
}
