namespace QuintasApp.Domain.Entities;

public class AlertaDisponibilidad
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = default!;
    public Guid QuintaId { get; private set; }
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }
    public string Email { get; private set; } = default!;
    public bool Notificado { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private AlertaDisponibilidad() { }

    public static AlertaDisponibilidad Crear(string userId, Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, string email)
    {
        return new AlertaDisponibilidad
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            QuintaId = quintaId,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Email = email.Trim(),
            Notificado = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarcarNotificado()
    {
        Notificado = true;
    }
}
