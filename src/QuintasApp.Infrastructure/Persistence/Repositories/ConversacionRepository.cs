using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class ConversacionRepository(MongoDbContext db) : IConversacionRepository
{
    public async Task<Conversacion?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var idStr = id.ToString();
        var doc = await db.Conversaciones.Find(c => c.Id == idStr).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToEntity(doc);
    }

    public async Task<List<Conversacion>> GetByQuintaAsync(Guid quintaId, CancellationToken ct = default)
    {
        var qId = quintaId.ToString();
        var docs = await db.Conversaciones
            .Find(c => c.QuintaId == qId)
            .SortByDescending(c => c.UltimoMensajeEn)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task<List<Conversacion>> GetByClienteAsync(string clienteId, CancellationToken ct = default)
    {
        var docs = await db.Conversaciones
            .Find(c => c.ClienteId == clienteId)
            .SortByDescending(c => c.UltimoMensajeEn)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task<Conversacion?> GetByQuintaYClienteAsync(Guid quintaId, string clienteId, CancellationToken ct = default)
    {
        var qId = quintaId.ToString();
        var doc = await db.Conversaciones
            .Find(c => c.QuintaId == qId && c.ClienteId == clienteId)
            .FirstOrDefaultAsync(ct);
        return doc is null ? null : ToEntity(doc);
    }

    public async Task AddAsync(Conversacion conversacion, CancellationToken ct = default)
    {
        await db.Conversaciones.InsertOneAsync(ToDocument(conversacion), cancellationToken: ct);
    }

    public async Task UpdateAsync(Conversacion conversacion, CancellationToken ct = default)
    {
        var idStr = conversacion.Id.ToString();
        await db.Conversaciones.ReplaceOneAsync(c => c.Id == idStr, ToDocument(conversacion), cancellationToken: ct);
    }

    private static Conversacion ToEntity(ConversacionDocument d)
    {
        var c = (Conversacion)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Conversacion));
        Set(c, "Id", Guid.Parse(d.Id));
        Set(c, "QuintaId", Guid.Parse(d.QuintaId));
        Set(c, "QuintaNombre", d.QuintaNombre);
        Set(c, "ClienteId", d.ClienteId);
        Set(c, "ClienteNombre", d.ClienteNombre);
        Set(c, "CreatedAt", d.CreatedAt);
        Set(c, "UltimoMensajeEn", d.UltimoMensajeEn);
        Set(c, "UltimoLeidoPorPropietario", d.UltimoLeidoPorPropietario);
        Set(c, "UltimoLeidoPorCliente", d.UltimoLeidoPorCliente);

        var mensajes = d.Mensajes.Select(m => new Mensaje(
            Guid.Parse(m.Id),
            m.Texto,
            Enum.Parse<RemitenteRol>(m.RemitenteRol),
            m.RemitenteId,
            m.EnviadoEn)).ToList();
        // Set the backing field _mensajes via reflection
        var field = typeof(Conversacion).GetField("_mensajes",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(c, mensajes);

        return c;
    }

    private static ConversacionDocument ToDocument(Conversacion c) => new()
    {
        Id = c.Id.ToString(),
        QuintaId = c.QuintaId.ToString(),
        QuintaNombre = c.QuintaNombre,
        ClienteId = c.ClienteId,
        ClienteNombre = c.ClienteNombre,
        CreatedAt = c.CreatedAt,
        UltimoMensajeEn = c.UltimoMensajeEn,
        UltimoLeidoPorPropietario = c.UltimoLeidoPorPropietario,
        UltimoLeidoPorCliente = c.UltimoLeidoPorCliente,
        Mensajes = c.Mensajes.Select(m => new MensajeDocument
        {
            Id = m.Id.ToString(),
            Texto = m.Texto,
            RemitenteRol = m.RemitenteRol.ToString(),
            RemitenteId = m.RemitenteId,
            EnviadoEn = m.EnviadoEn
        }).ToList()
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var p = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        p?.SetValue(obj, value);
    }
}
