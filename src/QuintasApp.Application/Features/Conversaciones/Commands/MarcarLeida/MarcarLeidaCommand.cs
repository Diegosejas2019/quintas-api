using MediatR;
using QuintasApp.Domain.Enums;

namespace QuintasApp.Application.Features.Conversaciones.Commands.MarcarLeida;

public record MarcarLeidaCommand(Guid ConversacionId, string SolicitanteId, RemitenteRol SolicitanteRol) : IRequest;
