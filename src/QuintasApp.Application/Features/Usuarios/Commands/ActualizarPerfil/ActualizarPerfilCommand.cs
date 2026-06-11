using MediatR;
using QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;

namespace QuintasApp.Application.Features.Usuarios.Commands.ActualizarPerfil;

public record ActualizarPerfilCommand(
    string SupabaseId,
    string? Nombre,
    string? Telefono
) : IRequest<UsuarioDto>;
