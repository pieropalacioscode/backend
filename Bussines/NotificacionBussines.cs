using AutoMapper;
using DBModel.DB;
using IBussines;
using IRepository;
using Microsoft.AspNetCore.SignalR;
using Models.RequestResponse;
using Models.Shared.Hubs;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Bussines
{
    public class NotificacionBussines : INotificacionBussines
    {
        #region Declaracion de vcariables generales
        private readonly INotificacionRepository _INotificacionRepository;
        private readonly IMapper _Mapper;
        private readonly IKardexRepository _kardexRepository;
        private readonly IHubContext<NotificacionHub> _hubContext;

        #endregion

        #region constructor 
        public NotificacionBussines(
        IMapper mapper,
        IKardexRepository kardexRepository,
        INotificacionRepository notificacionRepository,
        IHubContext<NotificacionHub> hubContext)
        {
            _Mapper = mapper;
            _INotificacionRepository = notificacionRepository;
            _kardexRepository = kardexRepository;
            _hubContext = hubContext;
        }
        #endregion

        public NotificacionResponse Create(NotificacionRequest entity)
        {
            Notificacion au = _Mapper.Map<Notificacion>(entity);
            au = _INotificacionRepository.Create(au);
            NotificacionResponse res = _Mapper.Map<NotificacionResponse>(au);
            return res;
        }

        public List<NotificacionResponse> CreateMultiple(List<NotificacionRequest> request)
        {
            List<Notificacion> au = _Mapper.Map<List<Notificacion>>(request);
            au = _INotificacionRepository.InsertMultiple(au);
            List<NotificacionResponse> res = _Mapper.Map<List<NotificacionResponse>>(au);
            return res;
        }

        public int Delete(object id)
        {
            return _INotificacionRepository.Delete(id);
        }

        public int deleteMultipleItems(List<NotificacionRequest> request)
        {
            List<Notificacion> au = _Mapper.Map<List<Notificacion>>(request);
            int cantidad = _INotificacionRepository.DeleteMultipleItems(au);
            return cantidad;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public List<NotificacionResponse> getAll()
        {
            List<Notificacion> lsl = _INotificacionRepository.GetAll();
            List<NotificacionResponse> res = _Mapper.Map<List<NotificacionResponse>>(lsl);
            return res;
        }

        public List<NotificacionResponse> getAutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public NotificacionResponse getById(object id)
        {
            Notificacion au = _INotificacionRepository.GetById(id);
            NotificacionResponse res = _Mapper.Map<NotificacionResponse>(au);
            return res;
        }

        public NotificacionResponse Update(NotificacionRequest entity)
        {
            Notificacion au = _Mapper.Map<Notificacion>(entity);
            au = _INotificacionRepository.Update(au);
            NotificacionResponse res = _Mapper.Map<NotificacionResponse>(au);
            return res;
        }

        public List<NotificacionResponse> UpdateMultiple(List<NotificacionRequest> request)
        {
            List<Notificacion> au = _Mapper.Map<List<Notificacion>>(request);
            au = _INotificacionRepository.UpdateMultiple(au);
            List<NotificacionResponse> res = _Mapper.Map<List<NotificacionResponse>>(au);
            return res;
        }

        public async Task VerificarStockBajoYNotificar()
        {
            var librosConStock = _kardexRepository.GetLibrosConStock();

            foreach (var libro in librosConStock)
            {
                if (libro.Stock <= 5)
                {
                    var noti = _INotificacionRepository.GetUltimaNotificacionStock(libro.IdLibro);

                    if (noti == null)
                    {
                        // ✅ No existe noti aún, crearla
                        var nueva = new Notificacion
                        {
                            IdLibro = libro.IdLibro,
                            Tipo = "stock",
                            Mensaje = $"📉 El libro \"{libro.IdLibroNavigation?.Titulo ?? "Sin título"}\" tiene stock bajo ({libro.Stock}).",
                            Fecha = DateTime.Now,
                            Leido = false
                        };

                        _INotificacionRepository.Create(nueva);

                        await _hubContext.Clients.All.SendAsync("NuevaNotificacion", new
                        {
                            mensaje = nueva.Mensaje,
                            idLibro = libro.IdLibro
                        });
                    }
                    else if (noti.Leido)
                    {
                        // 🔄 Ya existe noti, pero estaba leída, volver a marcar como no leída y reenviar
                        noti.Leido = false;
                        noti.Fecha = DateTime.Now; // opcional
                        _INotificacionRepository.Update(noti);

                        await _hubContext.Clients.All.SendAsync("NuevaNotificacion", new
                        {
                            mensaje = noti.Mensaje,
                            idLibro = libro.IdLibro
                        });
                    }
                }
                else
                {
                    // ✅ Stock ya es suficiente, marcar cualquier noti activa como leída
                    _INotificacionRepository.MarcarComoLeida(libro.IdLibro);
                }
            }
        }


    }
}
