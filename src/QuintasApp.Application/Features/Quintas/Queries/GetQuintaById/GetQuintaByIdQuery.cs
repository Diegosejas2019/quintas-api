using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;

namespace QuintasApp.Application.Features.Quintas.Queries.GetQuintaById;

public record GetQuintaByIdQuery(Guid Id) : IRequest<QuintaDto?>;
