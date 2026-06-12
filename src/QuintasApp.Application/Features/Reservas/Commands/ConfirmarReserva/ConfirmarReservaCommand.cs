using MediatR;

namespace QuintasApp.Application.Features.Reservas.Commands.ConfirmarReserva;

public record ConfirmarReservaCommand(Guid Id) : IRequest;
