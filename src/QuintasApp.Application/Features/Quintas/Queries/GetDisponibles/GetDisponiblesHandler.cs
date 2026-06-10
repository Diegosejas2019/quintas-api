using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Queries.GetDisponibles;

public class GetDisponiblesHandler(IQuintaRepository repo) : IRequestHandler<GetDisponiblesQuery, EstefindeResponse>
{
    public async Task<EstefindeResponse> Handle(GetDisponiblesQuery query, CancellationToken ct)
    {
        var quintas = await repo.GetDisponiblesEstefindeAsync(
            query.FechaInicio,
            query.FechaFin.AddDays(1),
            null, null, null, null,
            ct);

        var dtos = quintas.Select(q => new QuintaDto(
            q.Id, q.Nombre, q.Descripcion, q.PrecioPorDia, q.Capacidad,
            q.Imagenes, q.Activa, q.Direccion, q.Latitud, q.Longitud,
            q.Pileta, q.Parrilla)).ToList();

        return new EstefindeResponse(query.FechaInicio, query.FechaFin, dtos.Count, dtos);
    }
}
