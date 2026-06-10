using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;

namespace QuintasApp.Domain.Interfaces;

public interface IReservaRepository
{
    Task<List<Reserva>> GetAllAsync(EstadoReserva? estado, Guid? quintaId, int page, int size, CancellationToken ct = default);
    Task<Reserva?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteSolapamientoAsync(Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, Guid? excludeReservaId = null, CancellationToken ct = default);
    Task<List<DateOnly>> GetFechasOcupadasAsync(Guid quintaId, int mes, int anio, CancellationToken ct = default);
    Task AddAsync(Reserva reserva, string quintaNombre, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task InsertarFechasOcupadasAsync(Guid reservaId, Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken ct = default);
    Task LiberarFechasOcupadasAsync(Guid reservaId, CancellationToken ct = default);
    Task RegistrarSenaAsync(Guid reservaId, Sena sena, EstadoReserva nuevoEstado, DateTimeOffset updatedAt, CancellationToken ct = default);
}
