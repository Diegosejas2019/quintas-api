using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Queries.GetQuintas;

public class GetQuintasHandler(IQuintaRepository repo) : IRequestHandler<GetQuintasQuery, List<QuintaDto>>
{
    public async Task<List<QuintaDto>> Handle(GetQuintasQuery query, CancellationToken ct)
    {
        var quintas = query.PropietarioId is not null
            ? await repo.GetAllByPropietarioAsync(query.PropietarioId, ct)
            : await repo.GetAllAsync(ct);
        return quintas.Select(q => new QuintaDto(q.Id, q.Nombre, q.Descripcion, q.PrecioPorDia, q.Capacidad, q.Imagenes, q.Activa, q.Direccion, q.Latitud, q.Longitud, q.Pileta, q.Parrilla, q.Amenities)).ToList();
    }
}
