using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Infrastructure.Services;

public class BackgroundNotificador(
    IServiceScopeFactory scopeFactory,
    ILogger<BackgroundNotificador> logger) : IBackgroundNotificador
{
    public void ProgramarNotificacionCancelacion(Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin)
    {
        // Task.Run crea un thread pool thread independiente del request.
        // El scope propio garantiza que el DbContext no sea el dispuesto por el request.
        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<INotificacionService>();
            try
            {
                await svc.NotificarCancelacionAsync(quintaId, fechaInicio, fechaFin, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error al enviar notificaciones de cancelación para Quinta {QuintaId} fechas {Inicio}-{Fin}",
                    quintaId, fechaInicio, fechaFin);
            }
        });
    }
}
