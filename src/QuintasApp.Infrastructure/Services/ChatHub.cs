using System.Collections.Concurrent;
using System.Text.Json;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Infrastructure.Services;

public interface ISseWriter
{
    bool IsConnected { get; }
    Task WriteEventAsync(string tipo, string json);
}

public class ChatHub : IChatHub
{
    private readonly ConcurrentDictionary<Guid, List<ISseWriter>> _conexiones = new();

    public void Suscribir(Guid conversacionId, ISseWriter writer)
    {
        var lista = _conexiones.GetOrAdd(conversacionId, _ => []);
        lock (lista) lista.Add(writer);
    }

    public void Desuscribir(Guid conversacionId, ISseWriter writer)
    {
        if (_conexiones.TryGetValue(conversacionId, out var lista))
            lock (lista) lista.Remove(writer);
    }

    public async Task EmitirMensajeAsync(Guid conversacionId, object mensajeDto)
    {
        await EmitirAsync(conversacionId, "mensaje", JsonSerializer.Serialize(mensajeDto));
    }

    public async Task EmitirLeidoAsync(Guid conversacionId, string quien, DateTime timestamp)
    {
        var payload = JsonSerializer.Serialize(new { quien, timestamp });
        await EmitirAsync(conversacionId, "leido", payload);
    }

    private async Task EmitirAsync(Guid conversacionId, string tipo, string json)
    {
        if (!_conexiones.TryGetValue(conversacionId, out var lista)) return;

        List<ISseWriter> snapshot;
        lock (lista) snapshot = [.. lista];

        await Task.WhenAll(snapshot.Select(w => w.WriteEventAsync(tipo, json)));

        lock (lista) lista.RemoveAll(w => !w.IsConnected);
    }
}
