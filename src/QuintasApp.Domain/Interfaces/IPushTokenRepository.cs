using QuintasApp.Domain.Entities;

namespace QuintasApp.Domain.Interfaces;

public interface IPushTokenRepository
{
    Task<List<PushToken>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task UpsertAsync(string userId, string token, string platform = "expo", CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
