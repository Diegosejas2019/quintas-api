using MediatR;
using QuintasApp.Application.Features.Usuarios.Commands.UpsertUsuario;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Usuarios.Queries.GetPerfil;

public class GetPerfilHandler(IUsuarioRepository repo) : IRequestHandler<GetPerfilQuery, UsuarioDto>
{
    public async Task<UsuarioDto> Handle(GetPerfilQuery query, CancellationToken ct)
    {
        var usuario = await repo.GetBySupabaseIdAsync(query.SupabaseId, ct)
            ?? throw new DomainException("Perfil de usuario no encontrado.");
        return UpsertUsuarioHandler.ToDto(usuario);
    }
}
