using MediatR;

namespace QuintasApp.Application.Features.Quintas.Commands.BloquearFechas;

public record BloquearFechasCommand(Guid QuintaId, List<DateOnly> Fechas, string PropietarioId) : IRequest;
