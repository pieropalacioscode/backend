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
    public class DetalleVentaRepository : GenericRepository<DetalleVenta>, IDetalleVentaRepository
    {


        public List<DetalleVenta> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DetalleVenta>> GetDetalleVentasByPersonaId(int idPersona)
        {
            var detalleVentas = await db.DetalleVentas
                                        .Include(dv => dv.IdVentasNavigation) 
                                        .Where(dv => dv.IdVentasNavigation.IdPersona == idPersona)
                                        .ToListAsync();
            return detalleVentas;
        }

        public async Task<List<ProductosMasVendidosResponse>> ObtenerProductosMasVendidosAsync(int mes, int anio)
        {
            var query = @"
                WITH VentasMes AS (
                    SELECT 
                        dv.idLibro, 
                        l.Titulo AS nombreProducto, 
                        dv.Cantidad
                    FROM Ventas v
                    INNER JOIN Detalle_Ventas dv ON v.id_Ventas = dv.id_Ventas
                    INNER JOIN Libro l ON dv.idLibro = l.idLibro
                    WHERE MONTH(v.Fecha_Venta) = @Mes
                      AND YEAR(v.Fecha_Venta) = @Anio
                )
                SELECT 
                    idLibro, 
                    nombreProducto, 
                    SUM(Cantidad) AS TotalVendidos
                FROM VentasMes
                GROUP BY idLibro, nombreProducto
                ORDER BY TotalVendidos DESC";

            var productosMasVendidos = new List<ProductosMasVendidosResponse>();

            using (var connection = db.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    // Parámetros
                    var paramMes = command.CreateParameter();
                    paramMes.ParameterName = "@Mes";
                    paramMes.Value = mes;
                    command.Parameters.Add(paramMes);

                    var paramAnio = command.CreateParameter();
                    paramAnio.ParameterName = "@Anio";
                    paramAnio.Value = anio;
                    command.Parameters.Add(paramAnio);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productosMasVendidos.Add(new ProductosMasVendidosResponse
                            {
                                ProductoId = reader.GetInt32(0),
                                NombreProducto = reader.GetString(1),
                                TotalVendidos = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }

            return productosMasVendidos;
        }

        public async Task<List<DetalleVenta>> ObtenerDetallesPorIdsVentasAsync(List<int> idsVentas)
        {
            return await dbSet
                .Where(d => idsVentas.Contains(d.IdVentas ?? 0))
                .ToListAsync();
        }

        public async Task<List<DetalleVentaResponse>> ObtenerDetallesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var detalles = await dbSet
                .Where(d => d.IdVentasNavigation.FechaVenta >= fechaInicio && d.IdVentasNavigation.FechaVenta < fechaFin)
                .Select(d => new DetalleVentaResponse
                {
                    IdDetalleVentas = d.IdDetalleVentas,
                    IdLibro = d.IdLibro,
                    NombreProducto = d.NombreProducto,
                    PrecioUnit = d.PrecioUnit,
                    Cantidad = d.Cantidad,
                    Importe = d.Importe,
                    IdVentas = d.IdVentas,
                    Estado = d.Estado,
                    Descuento = d.Descuento
                })
                .ToListAsync();

            return detalles;
        }
        public async Task<List<VentaResponsePago>> GetPago()
        {
            var pagos = await dbSet
                .Where(v => v.IdVentas != null)
                .GroupBy(v => v.IdVentasNavigation.TipoPago ?? "Sin Tipo")
                .Select(g => new VentaResponsePago
                {
                    TipoPago = g.Key,
                    TotalPrecio = g.Sum(x => x.IdVentasNavigation.TotalPrecio)
                })
                .ToListAsync();


            return pagos;
        }

        public async Task<List<VentaHistoricaDto>> GetVentasPorLibroAsync(int idLibro)
        {
            // dbSet aquí hace referencia a DbSet<DetalleVenta>
            var query = dbSet
                .Include(dv => dv.IdVentasNavigation) // Incluimos la navegación hacia Venta
                .Where(dv => dv.IdLibro == idLibro && dv.IdVentasNavigation.FechaVenta != null)
                .GroupBy(dv => dv.IdVentasNavigation.FechaVenta.Date)
                .Select(g => new VentaHistoricaDto
                {
                    Fecha = g.Key,
                    Cantidad = g.Sum(x => x.Cantidad ?? 0)
                });

            return await query.OrderBy(x => x.Fecha).ToListAsync();
        }


    }
}
