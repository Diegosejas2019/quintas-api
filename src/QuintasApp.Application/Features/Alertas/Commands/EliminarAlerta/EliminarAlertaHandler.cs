using MediatR;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Alertas.Commands.EliminarAlerta;

public class EliminarAlertaHandler(IAlertaDisponibilidadRepository repo) : IRequestHandler<EliminarAlertaCommand>
{
    public async Task Handle(EliminarAlertaCommand cmd, CancellationToken ct)
    {
        var alerta = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new DomainException($"Alerta con id {cmd.Id} no encontrada.");

        if (alerta.UserId != cmd.UserId)
            throw new DomainException("No tenés permiso para eliminar esta alerta.");

        await repo.DeleteAsync(alerta, ct);
        await repo.SaveChangesAsync(ct);
    }
}
