using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Alertas.Queries.GetAlertasByUser;

public class GetAlertasByUserHandler(IAlertaDisponibilidadRepository repo, IQuintaRepository quintas) : IRequestHandler<GetAlertasByUserQuery, List<AlertaDto>>
{
    public async Task<List<AlertaDto>> Handle(GetAlertasByUserQuery query, CancellationToken ct)
    {
        var alertas = await repo.GetByUserIdAsync(query.UserId, ct);
        var todasLasQuintas = await quintas.GetAllAsync(ct);
        var nombrePorId = todasLasQuintas.ToDictionary(q => q.Id, q => q.Nombre);

        return alertas.Select(a =>
        {
            var nombre = nombrePorId.TryGetValue(a.QuintaId, out var n) ? n : "Quinta eliminada";
            return new AlertaDto(a.Id, a.QuintaId, nombre, a.FechaInicio, a.FechaFin, a.Email, a.Notificado, a.CreatedAt);
        }).ToList();
    }
}
