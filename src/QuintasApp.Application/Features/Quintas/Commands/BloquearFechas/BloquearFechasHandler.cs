using MediatR;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Commands.BloquearFechas;

public class BloquearFechasHandler(IQuintaRepository repo) : IRequestHandler<BloquearFechasCommand>
{
    public async Task Handle(BloquearFechasCommand cmd, CancellationToken ct)
    {
        var quinta = await repo.GetByIdAsync(cmd.QuintaId, ct)
            ?? throw new DomainException($"Quinta con id {cmd.QuintaId} no encontrada.");
        if (quinta.PropietarioId != cmd.PropietarioId)
            throw new DomainException("No tenés permiso para modificar esta quinta.");
        quinta.SetFechasBloqueadas(cmd.Fechas);
        await repo.SaveChangesAsync(ct);
    }
}
