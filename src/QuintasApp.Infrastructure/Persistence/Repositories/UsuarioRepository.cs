using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class UsuarioRepository(MongoDbContext db) : IUsuarioRepository
{
    public async Task<Usuario> UpsertBySupabaseIdAsync(Usuario usuario, CancellationToken ct = default)
    {
        var existing = await db.Usuarios
            .Find(u => u.SupabaseId == usuario.SupabaseId)
            .FirstOrDefaultAsync(ct);

        if (existing is not null)
            return ToEntity(existing);

        await db.Usuarios.InsertOneAsync(ToDocument(usuario), cancellationToken: ct);
        return usuario;
    }

    public async Task<Usuario?> GetBySupabaseIdAsync(string supabaseId, CancellationToken ct = default)
    {
        var doc = await db.Usuarios.Find(u => u.SupabaseId == supabaseId).FirstOrDefaultAsync(ct);
        if (doc is null) return null;
        var entity = ToEntity(doc);
        _tracked.Add(entity);
        return entity;
    }

    public async Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var idStr = id.ToString();
        var doc = await db.Usuarios.Find(u => u.Id == idStr).FirstOrDefaultAsync(ct);
        return doc is null ? null : ToEntity(doc);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var u in _tracked)
        {
            var idStr = u.Id.ToString();
            await db.Usuarios.ReplaceOneAsync(x => x.Id == idStr, ToDocument(u), cancellationToken: ct);
        }
        _tracked.Clear();
    }

    public async Task<List<string>> GetFavoritosAsync(string supabaseId, CancellationToken ct = default)
    {
        var doc = await db.Usuarios.Find(u => u.SupabaseId == supabaseId).FirstOrDefaultAsync(ct);
        return doc?.Favoritos ?? [];
    }

    public async Task<List<string>> SyncFavoritosAsync(string supabaseId, IEnumerable<string> quintaIds, CancellationToken ct = default)
    {
        var ids = quintaIds.Distinct().ToList();
        var update = Builders<UsuarioDocument>.Update.Set(u => u.Favoritos, ids);
        await db.Usuarios.UpdateOneAsync(u => u.SupabaseId == supabaseId, update, cancellationToken: ct);
        return ids;
    }

    private readonly List<Usuario> _tracked = [];

    public void Track(Usuario u) => _tracked.Add(u);

    private static Usuario ToEntity(UsuarioDocument d)
    {
        var u = (Usuario)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Usuario));
        Set(u, "Id", Guid.Parse(d.Id));
        Set(u, "SupabaseId", d.SupabaseId);
        Set(u, "Email", d.Email);
        Set(u, "Nombre", d.Nombre);
        Set(u, "Telefono", d.Telefono);
        Set(u, "TipoUsuario", d.TipoUsuario);
        Set(u, "CreatedAt", d.CreatedAt);
        Set(u, "UpdatedAt", d.UpdatedAt);
        Set(u, "Favoritos", d.Favoritos ?? []);
        return u;
    }

    private static UsuarioDocument ToDocument(Usuario u) => new()
    {
        Id = u.Id.ToString(),
        SupabaseId = u.SupabaseId,
        Email = u.Email,
        Nombre = u.Nombre,
        Telefono = u.Telefono,
        TipoUsuario = u.TipoUsuario,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt,
        Favoritos = u.Favoritos,
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var p = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        p?.SetValue(obj, value);
    }
}
