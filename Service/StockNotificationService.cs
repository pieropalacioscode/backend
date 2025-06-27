using IBussines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class StockNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StockNotificationService> _logger;

        public StockNotificationService(IServiceProvider serviceProvider, ILogger<StockNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var notiService = scope.ServiceProvider.GetRequiredService<INotificacionBussines>();

                try
                {
                    await notiService.VerificarStockBajoYNotificar();
                    _logger.LogInformation("Verificación de stock ejecutada.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al verificar stock.");
                }

                // Espera 10 minutos
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
