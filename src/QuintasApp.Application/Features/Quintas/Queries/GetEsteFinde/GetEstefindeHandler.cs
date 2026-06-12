using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;

public class GetEstefindeHandler(IQuintaRepository repo) : IRequestHandler<GetEstefindeQuery, EstefindeResponse>
{
    public async Task<EstefindeResponse> Handle(GetEstefindeQuery query, CancellationToken ct)
    {
        DateOnly inicio, fin;

        if (query.FechaInicio.HasValue && query.FechaFin.HasValue)
        {
            inicio = query.FechaInicio.Value;
            fin    = query.FechaFin.Value;
        }
        else
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            inicio = today.DayOfWeek switch
            {
                DayOfWeek.Saturday => today.AddDays(-1),
                DayOfWeek.Sunday   => today.AddDays(-2),
                _                  => today.AddDays(((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7)
            };
            fin = inicio.AddDays(2);
        }

        var lunesExclusive = fin.AddDays(1); // FechaFin es exclusiva en el modelo

        var quintas = await repo.GetDisponiblesEstefindeAsync(
            inicio, lunesExclusive,
            query.Capacidad, query.PrecioMax, query.Pileta, query.Parrilla,
            ct);

        var dtos = quintas.Select(q => new QuintaDto(
            q.Id, q.Nombre, q.Descripcion, q.PrecioPorDia, q.Capacidad,
            q.Imagenes, q.Activa, q.Direccion, q.Latitud, q.Longitud,
            q.Pileta, q.Parrilla, q.Amenities)).ToList();

        return new EstefindeResponse(inicio, fin, dtos.Count, dtos);
    }
}
