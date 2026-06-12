using MediatR;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Commands.ConfirmarReserva;

public class ConfirmarReservaHandler(IReservaRepository repo) : IRequestHandler<ConfirmarReservaCommand>
{
    public async Task Handle(ConfirmarReservaCommand cmd, CancellationToken ct)
    {
        var reserva = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new DomainException($"Reserva con id {cmd.Id} no encontrada.");

        reserva.Confirmar();
        await repo.SaveChangesAsync(ct);
    }
}
