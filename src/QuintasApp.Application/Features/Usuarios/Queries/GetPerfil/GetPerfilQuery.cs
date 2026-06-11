using MediatR;
using QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;

namespace QuintasApp.Application.Features.Usuarios.Queries.GetPerfil;

public record GetPerfilQuery(string SupabaseId) : IRequest<UsuarioDto>;
