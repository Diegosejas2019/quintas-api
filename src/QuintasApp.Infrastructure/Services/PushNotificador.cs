using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Infrastructure.Services;

public class PushNotificador(ExpoPushService expo) : IPushNotificador
{
    public Task EnviarAsync(IEnumerable<string> tokens, string titulo, string cuerpo, CancellationToken ct = default)
        => expo.SendAsync(tokens, titulo, cuerpo, ct);
}
