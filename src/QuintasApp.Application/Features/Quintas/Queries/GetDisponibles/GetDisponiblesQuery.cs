using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;

namespace QuintasApp.Application.Features.Quintas.Queries.GetDisponibles;

public record GetDisponiblesQuery(
    DateOnly FechaInicio,
    DateOnly FechaFin
) : IRequest<EstefindeResponse>;
