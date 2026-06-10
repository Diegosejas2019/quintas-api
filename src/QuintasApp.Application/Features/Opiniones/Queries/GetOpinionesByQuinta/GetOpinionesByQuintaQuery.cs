using MediatR;

namespace QuintasApp.Application.Features.Opiniones.Queries.GetOpinionesByQuinta;

public record OpinionDto(
    Guid Id,
    string NombreCliente,
    int Calificacion,
    string? Comentario,
    DateTimeOffset CreatedAt
);

public record GetOpinionesByQuintaResult(List<OpinionDto> Opiniones, double Promedio);

public record GetOpinionesByQuintaQuery(Guid QuintaId) : IRequest<GetOpinionesByQuintaResult>;
