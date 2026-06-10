using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;

namespace QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;

public record GetEstefindeQuery(
    int? Capacidad = null,
    decimal? PrecioMax = null,
    bool? Pileta = null,
    bool? Parrilla = null
) : IRequest<EstefindeResponse>;

public record EstefindeResponse(
    DateOnly Viernes,
    DateOnly Domingo,
    int Total,
    List<QuintaDto> Quintas
);
