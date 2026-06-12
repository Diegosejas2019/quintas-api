using MediatR;
using QuintasApp.Application.Features.Conversaciones;
using QuintasApp.Domain.Enums;

namespace QuintasApp.Application.Features.Conversaciones.Commands.EnviarMensaje;

public record EnviarMensajeCommand(
    Guid ConversacionId,
    string RemitenteId,
    RemitenteRol RemitenteRol,
    string Texto
) : IRequest<MensajeDto>;
