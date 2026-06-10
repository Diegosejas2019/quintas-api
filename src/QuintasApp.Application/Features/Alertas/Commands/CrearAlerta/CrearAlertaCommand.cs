using MediatR;

namespace QuintasApp.Application.Features.Alertas.Commands.CrearAlerta;

public record CrearAlertaCommand(
    string UserId,
    Guid QuintaId,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    string Email
) : IRequest<Guid>;
