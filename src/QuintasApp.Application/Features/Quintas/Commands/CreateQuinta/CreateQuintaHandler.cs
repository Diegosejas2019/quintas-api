using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Commands.CreateQuinta;

public class CreateQuintaHandler(IQuintaRepository repo) : IRequestHandler<CreateQuintaCommand, Guid>
{
    public async Task<Guid> Handle(CreateQuintaCommand cmd, CancellationToken ct)
    {
        var quinta = Quinta.Crear(cmd.Nombre, cmd.Descripcion, cmd.PrecioPorDia, cmd.Capacidad, cmd.Imagenes, cmd.Direccion, pileta: cmd.Pileta, parrilla: cmd.Parrilla, amenities: cmd.Amenities);
        await repo.AddAsync(quinta, ct);
        await repo.SaveChangesAsync(ct);
        return quinta.Id;
    }
}
