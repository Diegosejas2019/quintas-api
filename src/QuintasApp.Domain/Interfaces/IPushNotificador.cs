namespace QuintasApp.Domain.Interfaces;

public interface IPushNotificador
{
    Task EnviarAsync(IEnumerable<string> tokens, string titulo, string cuerpo, CancellationToken ct = default);
}
