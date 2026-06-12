using MediatR;
using QuintasApp.Application.Features.Conversaciones;
using QuintasApp.Domain.Enums;

namespace QuintasApp.Application.Features.Conversaciones.Queries.GetMensajes;

public record GetMensajesQuery(Guid ConversacionId, string SolicitanteId, RemitenteRol SolicitanteRol) : IRequest<List<MensajeDto>>;
