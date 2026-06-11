namespace QuintasApp.Domain.Entities;

public class Usuario
{
    public Guid Id { get; private set; }
    public string SupabaseId { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Nombre { get; private set; } = default!;
    public string? Telefono { get; private set; }
    public string TipoUsuario { get; private set; } = "cliente";
    public List<string> Favoritos { get; private set; } = [];
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Usuario() { }

    public static Usuario Crear(string supabaseId, string email, string nombre) => new()
    {
        Id = Guid.NewGuid(),
        SupabaseId = supabaseId,
        Email = email.Trim().ToLowerInvariant(),
        Nombre = nombre.Trim(),
        TipoUsuario = "cliente",
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    public void SyncFavoritos(IEnumerable<string> quintaIds)
    {
        Favoritos = quintaIds.Distinct().ToList();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ActualizarPerfil(string? nombre, string? telefono)
    {
        if (nombre is not null) Nombre = nombre.Trim();
        if (telefono is not null) Telefono = telefono.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
