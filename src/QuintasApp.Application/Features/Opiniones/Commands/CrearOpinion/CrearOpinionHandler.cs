using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Opiniones.Commands.CrearOpinion;

public class CrearOpinionHandler(IOpinionRepository repo) : IRequestHandler<CrearOpinionCommand, Guid>
{
    public async Task<Guid> Handle(CrearOpinionCommand cmd, CancellationToken ct)
    {
        var opinion = Opinion.Crear(cmd.QuintaId, cmd.UserId, cmd.NombreCliente, cmd.Calificacion, cmd.Comentario);
        await repo.AddAsync(opinion, ct);
        await repo.SaveChangesAsync(ct);
        return opinion.Id;
    }
}
