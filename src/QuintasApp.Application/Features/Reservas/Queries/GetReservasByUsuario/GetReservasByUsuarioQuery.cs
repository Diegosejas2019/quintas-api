using MediatR;
using QuintasApp.Application.Features.Reservas.Queries.GetReservas;

namespace QuintasApp.Application.Features.Reservas.Queries.GetReservasByUsuario;

public record GetReservasByUsuarioQuery(string UsuarioId) : IRequest<List<ReservaDto>>;
