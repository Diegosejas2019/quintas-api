using MediatR;

namespace QuintasApp.Application.Features.Usuarios.Queries.GetFavoritos;

public record GetFavoritosQuery(string SupabaseId) : IRequest<List<string>>;
