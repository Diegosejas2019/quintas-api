using MediatR;
using Microsoft.Extensions.Logging;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Commands.CancelReserva;

public class CancelReservaHandler(
    IReservaRepository repo,
    INotificacionService notificacionService,
    ILogger<CancelReservaHandler> logger) : IRequestHandler<CancelReservaCommand>
{
    public async Task Handle(CancelReservaCommand cmd, CancellationToken ct)
    {
        var reserva = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new DomainException($"Reserva con id {cmd.Id} no encontrada.");

        var quintaId = reserva.QuintaId;
        var fechaInicio = reserva.FechaInicio;
        var fechaFin = reserva.FechaFin;
        var reservaId = reserva.Id;

        reserva.Cancelar();
        await repo.SaveChangesAsync(ct);
        await repo.LiberarFechasOcupadasAsync(reservaId, ct);

        try
        {
            await notificacionService.NotificarCancelacionAsync(quintaId, fechaInicio, fechaFin, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al notificar cancelación de Quinta {QuintaId}", quintaId);
        }
    }
}
