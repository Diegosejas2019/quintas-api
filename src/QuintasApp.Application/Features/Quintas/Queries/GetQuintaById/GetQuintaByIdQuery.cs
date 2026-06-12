using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;

namespace QuintasApp.Application.Features.Quintas.Queries.GetQuintaById;

public record GetQuintaByIdQuery(Guid Id, string? ClienteId = null) : IRequest<QuintaDto?>;
