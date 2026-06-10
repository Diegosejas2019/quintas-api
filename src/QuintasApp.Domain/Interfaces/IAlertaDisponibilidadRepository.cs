using QuintasApp.Domain.Entities;

namespace QuintasApp.Domain.Interfaces;

public interface IAlertaDisponibilidadRepository
{
    Task<List<AlertaDisponibilidad>> GetActivasByQuintaIdAsync(Guid quintaId, CancellationToken ct = default);
    Task<List<AlertaDisponibilidad>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<AlertaDisponibilidad?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(AlertaDisponibilidad alerta, CancellationToken ct = default);
    Task DeleteAsync(AlertaDisponibilidad alerta, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
