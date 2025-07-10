using DBModel.DB;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class KardexRepository : GenericRepository<Kardex>, IKardexRepository
    {
        public List<Kardex> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }
        public void RegistrarEntradaKardex(int idLibro, int idSucursal, int cantidad, decimal precioUnitario)
        {
            // Buscar el registro existente
            var kardexExistente = db.Kardices
                .FirstOrDefault(k => k.IdLibro == idLibro && k.IdSucursal == idSucursal);

            if (kardexExistente != null)
            {
                // Actualizar stock acumulado
                kardexExistente.Stock += cantidad;

                // Actualizar última entrada registrada
                kardexExistente.CantidadEntrada = cantidad;

                // Actualizar último precio de costo
                kardexExistente.UltPrecioCosto = precioUnitario;

                db.Kardices.Update(kardexExistente);
            }
            else
            {
                // Crear nuevo registro si no existe
                var nuevoMovimiento = new Kardex
                {
                    IdLibro = idLibro,
                    IdSucursal = idSucursal,
                    CantidadEntrada = cantidad,
                    CantidadSalida = 0,
                    Stock = cantidad,
                    UltPrecioCosto = precioUnitario,
                };

                db.Kardices.Add(nuevoMovimiento);
            }

            db.SaveChanges();
        }


        public List<Kardex> GetLibrosConStock()
        {
            return dbSet
                .Include(k => k.IdLibroNavigation)
                .Where(k => k.Stock != null)
                .ToList();
        }

        public async Task<Kardex> GetByIdAsync(int id)
        {
            return await dbSet.FirstOrDefaultAsync(k => k.IdLibro == id);
        }

    }
}
