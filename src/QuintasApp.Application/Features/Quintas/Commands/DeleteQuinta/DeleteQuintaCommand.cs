using MediatR;

namespace QuintasApp.Application.Features.Quintas.Commands.DeleteQuinta;

public record DeleteQuintaCommand(Guid Id) : IRequest;
