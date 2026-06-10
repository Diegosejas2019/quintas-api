using QuintasApp.Domain.Entities;

namespace QuintasApp.Domain.Interfaces;

public interface IOpinionRepository
{
    Task<List<Opinion>> GetByQuintaIdAsync(Guid quintaId, CancellationToken ct = default);
    Task AddAsync(Opinion opinion, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
