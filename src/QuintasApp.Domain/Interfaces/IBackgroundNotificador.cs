namespace QuintasApp.Domain.Interfaces;

/// <summary>
/// Encola notificaciones para envío en background.
/// La implementación crea su propio scope de DI para que el DbContext
/// no sea dispuesto al completar el HTTP request.
/// </summary>
public interface IBackgroundNotificador
{
    void ProgramarNotificacionCancelacion(Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin);
}
