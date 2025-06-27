using DBModel.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilInterface;

namespace IRepository
{
    public interface INotificacionRepository : ICRUDRepositorio<Notificacion>
    {
        bool ExisteNotificacionStockHoy(int idLibro);
        Notificacion GetUltimaNotificacionStock(int idLibro);
        void MarcarComoLeida(int idLibro);

    }
}
