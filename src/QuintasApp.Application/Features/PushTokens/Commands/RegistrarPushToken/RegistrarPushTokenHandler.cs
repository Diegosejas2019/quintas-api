using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.PushTokens.Commands.RegistrarPushToken;

public class RegistrarPushTokenHandler(IPushTokenRepository repo) : IRequestHandler<RegistrarPushTokenCommand>
{
    public async Task Handle(RegistrarPushTokenCommand cmd, CancellationToken ct)
    {
        await repo.UpsertAsync(cmd.UserId, cmd.Token, ct);
        await repo.SaveChangesAsync(ct);
    }
}
