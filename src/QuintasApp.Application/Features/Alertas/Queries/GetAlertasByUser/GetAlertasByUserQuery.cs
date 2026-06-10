using MediatR;

namespace QuintasApp.Application.Features.Alertas.Queries.GetAlertasByUser;

public record AlertaDto(
    Guid Id,
    Guid QuintaId,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string Email,
    bool Notificado,
    DateTimeOffset CreatedAt
);

public record GetAlertasByUserQuery(string UserId) : IRequest<List<AlertaDto>>;
