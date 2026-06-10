using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class OpinionRepository(MongoDbContext db) : IOpinionRepository
{
    public async Task<List<Opinion>> GetByQuintaIdAsync(Guid quintaId, CancellationToken ct = default)
    {
        var qIdStr = quintaId.ToString();
        var docs = await db.Opiniones
            .Find(o => o.QuintaId == qIdStr)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Opinion opinion, CancellationToken ct = default) =>
        await db.Opiniones.InsertOneAsync(ToDocument(opinion), cancellationToken: ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;

    private static Opinion ToEntity(OpinionDocument d)
    {
        var o = (Opinion)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Opinion));
        Set(o, "Id", Guid.Parse(d.Id));
        Set(o, "QuintaId", Guid.Parse(d.QuintaId));
        Set(o, "UserId", d.UserId);
        Set(o, "NombreCliente", d.NombreCliente);
        Set(o, "Calificacion", d.Calificacion);
        Set(o, "Comentario", d.Comentario);
        Set(o, "CreatedAt", d.CreatedAt);
        return o;
    }

    private static OpinionDocument ToDocument(Opinion o) => new()
    {
        Id = o.Id.ToString(),
        QuintaId = o.QuintaId.ToString(),
        UserId = o.UserId,
        NombreCliente = o.NombreCliente,
        Calificacion = o.Calificacion,
        Comentario = o.Comentario,
        CreatedAt = o.CreatedAt,
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var p = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        p?.SetValue(obj, value);
    }
}
