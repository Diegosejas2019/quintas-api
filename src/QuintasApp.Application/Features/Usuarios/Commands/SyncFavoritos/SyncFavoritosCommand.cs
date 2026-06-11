using MediatR;

namespace QuintasApp.Application.Features.Usuarios.Commands.SyncFavoritos;

public record SyncFavoritosCommand(string SupabaseId, List<string> QuintaIds) : IRequest<List<string>>;
