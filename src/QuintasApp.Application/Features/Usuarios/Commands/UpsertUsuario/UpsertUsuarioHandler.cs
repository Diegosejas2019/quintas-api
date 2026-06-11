using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;

public class UpsertUsuarioHandler(IUsuarioRepository repo) : IRequestHandler<UpsertUsuarioCommand, UsuarioDto>
{
    public async Task<UsuarioDto> Handle(UpsertUsuarioCommand cmd, CancellationToken ct)
    {
        var nuevo = Usuario.Crear(cmd.SupabaseId, cmd.Email, cmd.Nombre);
        var usuario = await repo.UpsertBySupabaseIdAsync(nuevo, ct);
        return ToDto(usuario);
    }

    internal static UsuarioDto ToDto(Usuario u) =>
        new(u.Id, u.Email, u.Nombre, u.Telefono, u.TipoUsuario);
}
