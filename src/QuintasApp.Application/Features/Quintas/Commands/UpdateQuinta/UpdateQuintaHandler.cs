using MediatR;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Commands.UpdateQuinta;

public class UpdateQuintaHandler(IQuintaRepository repo) : IRequestHandler<UpdateQuintaCommand>
{
    public async Task Handle(UpdateQuintaCommand cmd, CancellationToken ct)
    {
        var quinta = await repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new DomainException($"Quinta con id {cmd.Id} no encontrada.");
        quinta.Actualizar(cmd.Nombre, cmd.Descripcion, cmd.PrecioPorDia, cmd.Capacidad, cmd.Imagenes, cmd.Direccion, pileta: cmd.Pileta, parrilla: cmd.Parrilla, amenities: cmd.Amenities);
        await repo.SaveChangesAsync(ct);
    }
}
