using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QuintasApp.Infrastructure.Services;

public class ResendEmailService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<ResendEmailService> logger)
{
    public async Task SendAlertaDisponibilidadAsync(string toEmail, string quintaNombre, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken ct = default)
    {
        var fromEmail = config["Resend:FromEmail"] ?? "noreply@quintasapp.com";
        var body = new
        {
            from = fromEmail,
            to = new[] { toEmail },
            subject = $"¡Disponibilidad liberada! {quintaNombre}",
            html = $"""
                <h2>¡Buenas noticias!</h2>
                <p>La quinta <strong>{quintaNombre}</strong> tiene disponibilidad para las fechas que solicitaste:</p>
                <p><strong>{fechaInicio:dd/MM/yyyy} — {fechaFin:dd/MM/yyyy}</strong></p>
                <p>Ingresá a la app para reservar antes de que se ocupe.</p>
                <br/>
                <p>Quintas App</p>
                """
        };

        try
        {
            var client = httpClientFactory.CreateClient("Resend");
            var response = await client.PostAsJsonAsync("https://api.resend.com/emails", body, ct);
            if (!response.IsSuccessStatusCode)
                logger.LogWarning("Resend devolvió {Status} para {Email}", response.StatusCode, toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enviando email a {Email}", toEmail);
        }
    }
}
