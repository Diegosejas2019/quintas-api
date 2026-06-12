using QuintasApp.Domain.Entities;

namespace QuintasApp.Domain.Interfaces;

public interface IConversacionRepository
{
    Task<Conversacion?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Conversacion>> GetByQuintaAsync(Guid quintaId, CancellationToken ct = default);
    Task<List<Conversacion>> GetByClienteAsync(string clienteId, CancellationToken ct = default);
    Task<Conversacion?> GetByQuintaYClienteAsync(Guid quintaId, string clienteId, CancellationToken ct = default);
    Task AddAsync(Conversacion conversacion, CancellationToken ct = default);
    Task UpdateAsync(Conversacion conversacion, CancellationToken ct = default);
}
