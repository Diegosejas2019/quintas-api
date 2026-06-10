using MediatR;

namespace QuintasApp.Application.Features.Reservas.Commands.CancelReserva;

public record CancelReservaCommand(Guid Id) : IRequest;
