using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Usuarios.Queries.GetFavoritos;

public class GetFavoritosHandler(IUsuarioRepository repo) : IRequestHandler<GetFavoritosQuery, List<string>>
{
    public async Task<List<string>> Handle(GetFavoritosQuery query, CancellationToken ct)
        => await repo.GetFavoritosAsync(query.SupabaseId, ct);
}
