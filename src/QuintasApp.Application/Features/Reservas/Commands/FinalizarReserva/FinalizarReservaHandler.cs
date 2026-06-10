using MediatR;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Commands.FinalizarReserva;

public class FinalizarReservaHandler(IReservaRepository repo) : IRequestHandler<FinalizarReservaCommand>
{
    public async Task Handle(FinalizarReservaCommand cmd, CancellationToken ct)
    {
        var reserva = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new DomainException($"Reserva con id {cmd.Id} no encontrada.");
        reserva.Finalizar();
        await repo.SaveChangesAsync(ct);
    }
}
