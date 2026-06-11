using MediatR;
using QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Usuarios.Commands.ActualizarPerfil;

public class ActualizarPerfilHandler(IUsuarioRepository repo) : IRequestHandler<ActualizarPerfilCommand, UsuarioDto>
{
    public async Task<UsuarioDto> Handle(ActualizarPerfilCommand cmd, CancellationToken ct)
    {
        var usuario = await repo.GetBySupabaseIdAsync(cmd.SupabaseId, ct)
            ?? throw new DomainException("Perfil de usuario no encontrado.");

        usuario.ActualizarPerfil(cmd.Nombre, cmd.Telefono);
        await repo.SaveChangesAsync(ct);
        return UpsertUsuarioHandler.ToDto(usuario);
    }
}
