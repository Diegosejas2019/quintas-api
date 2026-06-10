using MediatR;

namespace QuintasApp.Application.Features.Reservas.Queries.GetDisponibilidad;

public record GetDisponibilidadQuery(Guid QuintaId, int Mes, int Anio) : IRequest<List<string>>;
