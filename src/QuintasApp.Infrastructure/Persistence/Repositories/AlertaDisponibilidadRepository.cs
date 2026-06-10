using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class AlertaDisponibilidadRepository(MongoDbContext db) : IAlertaDisponibilidadRepository
{
    public async Task<List<AlertaDisponibilidad>> GetActivasByQuintaIdAsync(Guid quintaId, CancellationToken ct = default)
    {
        var qIdStr = quintaId.ToString();
        var docs = await db.AlertasDisponibilidad
            .Find(a => a.QuintaId == qIdStr && !a.Notificado)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task<List<AlertaDisponibilidad>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var docs = await db.AlertasDisponibilidad
            .Find(a => a.UserId == userId)
            .SortByDescending(a => a.CreatedAt)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task<AlertaDisponibilidad?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var idStr = id.ToString();
        var doc = await db.AlertasDisponibilidad.Find(a => a.Id == idStr).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToEntity(doc);
    }

    public async Task AddAsync(AlertaDisponibilidad alerta, CancellationToken ct = default) =>
        await db.AlertasDisponibilidad.InsertOneAsync(ToDocument(alerta), cancellationToken: ct);

    public async Task DeleteAsync(AlertaDisponibilidad alerta, CancellationToken ct = default)
    {
        var idStr = alerta.Id.ToString();
        await db.AlertasDisponibilidad.DeleteOneAsync(a => a.Id == idStr, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;

    private static AlertaDisponibilidad ToEntity(AlertaDocument d)
    {
        var a = (AlertaDisponibilidad)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(AlertaDisponibilidad));
        Set(a, "Id", Guid.Parse(d.Id));
        Set(a, "UserId", d.UserId);
        Set(a, "QuintaId", Guid.Parse(d.QuintaId));
        Set(a, "FechaInicio", DateOnly.ParseExact(d.FechaInicio, "yyyy-MM-dd"));
        Set(a, "FechaFin", DateOnly.ParseExact(d.FechaFin, "yyyy-MM-dd"));
        Set(a, "Email", d.Email);
        Set(a, "Notificado", d.Notificado);
        Set(a, "CreatedAt", d.CreatedAt);
        return a;
    }

    private static AlertaDocument ToDocument(AlertaDisponibilidad a) => new()
    {
        Id = a.Id.ToString(),
        UserId = a.UserId,
        QuintaId = a.QuintaId.ToString(),
        FechaInicio = a.FechaInicio.ToString("yyyy-MM-dd"),
        FechaFin = a.FechaFin.ToString("yyyy-MM-dd"),
        Email = a.Email,
        Notificado = a.Notificado,
        CreatedAt = a.CreatedAt,
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var p = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        p?.SetValue(obj, value);
    }
}
