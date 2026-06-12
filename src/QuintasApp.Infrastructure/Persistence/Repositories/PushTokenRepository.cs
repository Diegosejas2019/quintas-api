using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class PushTokenRepository(MongoDbContext db) : IPushTokenRepository
{
    public async Task<List<PushToken>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var docs = await db.PushTokens.Find(p => p.UserId == userId).ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task UpsertAsync(string userId, string token, string platform = "expo", CancellationToken ct = default)
    {
        var exists = await db.PushTokens.Find(p => p.Token == token).AnyAsync(ct);
        if (!exists)
            await db.PushTokens.InsertOneAsync(ToDocument(PushToken.Crear(userId, token, platform)), cancellationToken: ct);
        else
            await db.PushTokens.UpdateOneAsync(
                p => p.Token == token,
                Builders<PushTokenDocument>.Update.Set(p => p.Platform, platform),
                cancellationToken: ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;

    private static PushToken ToEntity(PushTokenDocument d)
    {
        var p = (PushToken)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(PushToken));
        Set(p, "Id", Guid.Parse(d.Id));
        Set(p, "UserId", d.UserId);
        Set(p, "Token", d.Token);
        Set(p, "Platform", d.Platform);
        Set(p, "CreatedAt", d.CreatedAt);
        return p;
    }

    private static PushTokenDocument ToDocument(PushToken p) => new()
    {
        Id = p.Id.ToString(),
        UserId = p.UserId,
        Token = p.Token,
        Platform = p.Platform,
        CreatedAt = p.CreatedAt,
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var pi = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pi?.SetValue(obj, value);
    }
}
