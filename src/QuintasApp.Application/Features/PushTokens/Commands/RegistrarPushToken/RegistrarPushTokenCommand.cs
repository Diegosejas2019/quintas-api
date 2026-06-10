using MediatR;

namespace QuintasApp.Application.Features.PushTokens.Commands.RegistrarPushToken;

public record RegistrarPushTokenCommand(string UserId, string Token) : IRequest;
