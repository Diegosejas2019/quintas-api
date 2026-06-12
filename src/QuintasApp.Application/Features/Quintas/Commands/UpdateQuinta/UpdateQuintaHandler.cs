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
        if (quinta.PropietarioId != cmd.PropietarioId)
            throw new DomainException("No tenés permiso para modificar esta quinta.");
        quinta.Actualizar(cmd.Nombre, cmd.Descripcion, cmd.PrecioPorDia, cmd.Capacidad, cmd.Imagenes, cmd.Direccion, cmd.Latitud, cmd.Longitud, pileta: cmd.Pileta, parrilla: cmd.Parrilla, amenities: cmd.Amenities, horaInicio: cmd.HoraInicio, horaFin: cmd.HoraFin);
        await repo.SaveChangesAsync(ct);
    }
}
