using MediatR;

namespace QuintasApp.Application.Features.Alertas.Commands.EliminarAlerta;

public record EliminarAlertaCommand(Guid Id, string UserId) : IRequest;
