using MediatR;
using QuintasApp.Application.Features.Conversaciones;

namespace QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByCliente;

public record GetConversacionesByClienteQuery(string ClienteId) : IRequest<List<ConversacionDto>>;
