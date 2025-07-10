using DBModel.DB;
using DocumentFormat.OpenXml.InkML;
using IRepository;
using Repository.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class NotificacionRepository : GenericRepository<Notificacion>, INotificacionRepository
    {
        public List<Notificacion> GetAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public bool ExisteNotificacionStockHoy(int idLibro)
        {
            return dbSet.Any(n =>
                n.IdLibro == idLibro &&
                n.Tipo == "stock" &&
                n.Fecha.Date == DateTime.Now.Date);
        }

        public Notificacion GetUltimaNotificacionStock(int idLibro)
        {
            return dbSet
                .Where(n => n.IdLibro == idLibro && n.Tipo == "stock")
                .OrderByDescending(n => n.Fecha)
                .FirstOrDefault();
        }

        public void MarcarComoLeida(int idLibro)
        {
            var notis = dbSet
                .Where(n => n.IdLibro == idLibro && n.Tipo == "stock" && !n.Leido)
                .ToList();

            foreach (var n in notis)
            {
                n.Leido = true;
            }

            db.SaveChanges();
        }


    }
}
