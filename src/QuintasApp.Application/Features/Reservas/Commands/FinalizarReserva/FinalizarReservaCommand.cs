using MediatR;

namespace QuintasApp.Application.Features.Reservas.Commands.FinalizarReserva;

public record FinalizarReservaCommand(Guid Id) : IRequest;
