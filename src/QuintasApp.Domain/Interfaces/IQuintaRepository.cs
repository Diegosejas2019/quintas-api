using QuintasApp.Domain.Entities;

namespace QuintasApp.Domain.Interfaces;

public interface IQuintaRepository
{
    Task<List<Quinta>> GetAllAsync(CancellationToken ct = default);
    Task<List<Quinta>> GetAllByPropietarioAsync(string propietarioId, CancellationToken ct = default);
    Task<List<string>> GetIdsByPropietarioAsync(string propietarioId, CancellationToken ct = default);
    Task<Quinta?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Quinta>> GetDisponiblesEstefindeAsync(DateOnly viernes, DateOnly lunesExclusive, int? capacidad, decimal? precioMax, bool? pileta, bool? parrilla, CancellationToken ct = default);
    Task AddAsync(Quinta quinta, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
