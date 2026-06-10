using MediatR;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Commands.DeleteQuinta;

public class DeleteQuintaHandler(IQuintaRepository repo) : IRequestHandler<DeleteQuintaCommand>
{
    public async Task Handle(DeleteQuintaCommand cmd, CancellationToken ct)
    {
        var quinta = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new DomainException($"Quinta con id {cmd.Id} no encontrada.");
        quinta.Desactivar();
        await repo.SaveChangesAsync(ct);
    }
}
