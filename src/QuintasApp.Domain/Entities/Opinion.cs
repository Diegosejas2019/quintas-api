namespace QuintasApp.Domain.Entities;

public class Opinion
{
    public Guid Id { get; private set; }
    public Guid QuintaId { get; private set; }
    public string UserId { get; private set; } = default!;
    public string NombreCliente { get; private set; } = default!;
    public int Calificacion { get; private set; }
    public string? Comentario { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Opinion() { }

    public static Opinion Crear(Guid quintaId, string userId, string nombreCliente, int calificacion, string? comentario)
    {
        return new Opinion
        {
            Id = Guid.NewGuid(),
            QuintaId = quintaId,
            UserId = userId,
            NombreCliente = nombreCliente.Trim(),
            Calificacion = calificacion,
            Comentario = comentario?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
