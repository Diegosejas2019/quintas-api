using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace QuintasApp.Infrastructure.Services;

public class ExpoPushService(IHttpClientFactory httpClientFactory, ILogger<ExpoPushService> logger)
{
    public async Task SendAsync(IEnumerable<string> tokens, string title, string body, CancellationToken ct = default)
    {
        var messages = tokens.Select(token => new
        {
            to = token,
            title,
            body,
            sound = "default"
        }).ToList();

        if (messages.Count == 0) return;

        try
        {
            var client = httpClientFactory.CreateClient("ExpoPush");
            var response = await client.PostAsJsonAsync("https://exp.host/--/api/v2/push/send", messages, ct);
            if (!response.IsSuccessStatusCode)
                logger.LogWarning("Expo Push devolvió {Status}", response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enviando push notifications");
        }
    }
}
