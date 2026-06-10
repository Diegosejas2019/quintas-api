using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Opiniones.Queries.GetOpinionesByQuinta;

public class GetOpinionesByQuintaHandler(IOpinionRepository repo) : IRequestHandler<GetOpinionesByQuintaQuery, GetOpinionesByQuintaResult>
{
    public async Task<GetOpinionesByQuintaResult> Handle(GetOpinionesByQuintaQuery query, CancellationToken ct)
    {
        var opiniones = await repo.GetByQuintaIdAsync(query.QuintaId, ct);
        var dtos = opiniones.Select(o => new OpinionDto(o.Id, o.NombreCliente, o.Calificacion, o.Comentario, o.CreatedAt)).ToList();
        var promedio = dtos.Count > 0 ? dtos.Average(o => o.Calificacion) : 0;
        return new GetOpinionesByQuintaResult(dtos, Math.Round(promedio, 1));
    }
}
