using Microsoft.AspNetCore.Http;
using QuintasApp.Infrastructure.Services;

namespace QuintasApp.API;

public class SseClient(HttpResponse response) : ISseWriter
{
    public bool IsConnected { get; private set; } = true;

    public async Task WriteEventAsync(string tipo, string json)
    {
        try
        {
            await response.WriteAsync($"event: {tipo}\ndata: {json}\n\n");
            await response.Body.FlushAsync();
        }
        catch
        {
            IsConnected = false;
        }
    }
}
