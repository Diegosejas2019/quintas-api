namespace QuintasApp.Domain.Entities;

public class PushToken
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    private PushToken() { }

    public static PushToken Crear(string userId, string token)
    {
        return new PushToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
