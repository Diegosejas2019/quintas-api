using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Alertas.Queries.GetAlertasByUser;

public class GetAlertasByUserHandler(IAlertaDisponibilidadRepository repo) : IRequestHandler<GetAlertasByUserQuery, List<AlertaDto>>
{
    public async Task<List<AlertaDto>> Handle(GetAlertasByUserQuery query, CancellationToken ct)
    {
        var alertas = await repo.GetByUserIdAsync(query.UserId, ct);
        return alertas.Select(a => new AlertaDto(a.Id, a.QuintaId, a.FechaInicio, a.FechaFin, a.Email, a.Notificado, a.CreatedAt)).ToList();
    }
}
