using QuintasApp.Domain.Entities;

namespace QuintasApp.Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> UpsertBySupabaseIdAsync(Usuario usuario, CancellationToken ct = default);
    Task<Usuario?> GetBySupabaseIdAsync(string supabaseId, CancellationToken ct = default);
    Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
