using MediatR;

namespace QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;

public record UsuarioDto(Guid Id, string Email, string Nombre, string? Telefono, string TipoUsuario);

public record UpsertUsuarioCommand(
    string SupabaseId,
    string Email,
    string Nombre
) : IRequest<UsuarioDto>;
