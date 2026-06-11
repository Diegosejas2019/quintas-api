using MediatR;

namespace QuintasApp.Application.Features.Reservas.Commands.CreateReserva;

public record CreateReservaCommand(
    Guid QuintaId,
    string NombreCliente,
    string EmailCliente,
    string TelefonoCliente,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    Guid? UsuarioId = null
) : IRequest<Guid>;
