namespace QuintasApp.Domain.Interfaces;

public interface INotificacionService
{
    Task NotificarCancelacionAsync(Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken ct = default);
}
