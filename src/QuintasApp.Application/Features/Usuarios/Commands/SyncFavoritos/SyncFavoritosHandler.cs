using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Usuarios.Commands.SyncFavoritos;

public class SyncFavoritosHandler(IUsuarioRepository repo) : IRequestHandler<SyncFavoritosCommand, List<string>>
{
    public async Task<List<string>> Handle(SyncFavoritosCommand cmd, CancellationToken ct)
        => await repo.SyncFavoritosAsync(cmd.SupabaseId, cmd.QuintaIds, ct);
}
