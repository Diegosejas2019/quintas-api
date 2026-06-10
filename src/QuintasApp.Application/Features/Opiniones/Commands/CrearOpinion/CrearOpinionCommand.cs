using MediatR;

namespace QuintasApp.Application.Features.Opiniones.Commands.CrearOpinion;

public record CrearOpinionCommand(
    Guid QuintaId,
    string UserId,
    string NombreCliente,
    int Calificacion,
    string? Comentario
) : IRequest<Guid>;
