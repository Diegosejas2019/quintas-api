using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Senas.Commands.RegistrarSena;

public class RegistrarSenaHandler(IReservaRepository reservaRepo)
    : IRequestHandler<RegistrarSenaCommand, Guid>
{
    public async Task<Guid> Handle(RegistrarSenaCommand cmd, CancellationToken ct)
    {
        var reserva = await reservaRepo.GetByIdAsync(cmd.ReservaId, ct)
            ?? throw new DomainException($"Reserva con id {cmd.ReservaId} no encontrada.");

        if (reserva.Estado == EstadoReserva.Cancelada || reserva.Estado == EstadoReserva.Finalizada)
            throw new DomainException("No se puede registrar seña en una reserva cancelada o finalizada.");

        if (reserva.Sena != null)
            throw new DomainException("Esta reserva ya tiene una seña registrada.");

        var sena = Sena.Crear(cmd.ReservaId, cmd.Monto, cmd.Tipo, cmd.Porcentaje, cmd.FechaPago, cmd.MetodoPago, reserva.PrecioTotal);

        await reservaRepo.RegistrarSenaAsync(reserva.Id, sena, reserva.Estado, reserva.UpdatedAt, ct);

        return sena.Id;
    }
}
