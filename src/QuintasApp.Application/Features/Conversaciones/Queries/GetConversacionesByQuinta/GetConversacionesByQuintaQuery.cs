using MediatR;
using QuintasApp.Application.Features.Conversaciones;

namespace QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByQuinta;

public record GetConversacionesByQuintaQuery(Guid QuintaId, string PropietarioId) : IRequest<List<ConversacionDto>>;
