using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Alertas.Commands.CrearAlerta;

public class CrearAlertaHandler(IAlertaDisponibilidadRepository repo) : IRequestHandler<CrearAlertaCommand, Guid>
{
    public async Task<Guid> Handle(CrearAlertaCommand cmd, CancellationToken ct)
    {
        var alerta = AlertaDisponibilidad.Crear(cmd.UserId, cmd.QuintaId, cmd.FechaInicio, cmd.FechaFin, cmd.Email);
        await repo.AddAsync(alerta, ct);
        await repo.SaveChangesAsync(ct);
        return alerta.Id;
    }
}
