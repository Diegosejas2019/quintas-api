using MediatR;
using QuintasApp.Application.Features.Conversaciones;

namespace QuintasApp.Application.Features.Conversaciones.Commands.IniciarConversacion;

public record IniciarConversacionCommand(
    Guid QuintaId,
    string ClienteId,
    string ClienteNombre
) : IRequest<ConversacionDto>;
