using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Conversaciones.Commands.IniciarConversacion;

public class IniciarConversacionHandler(
    IConversacionRepository repo,
    IQuintaRepository quintas) : IRequestHandler<IniciarConversacionCommand, ConversacionDto>
{
    public async Task<ConversacionDto> Handle(IniciarConversacionCommand cmd, CancellationToken ct)
    {
        var quinta = await quintas.GetByIdAsync(cmd.QuintaId, ct)
            ?? throw new DomainException($"Quinta {cmd.QuintaId} no encontrada.");

        var existente = await repo.GetByQuintaYClienteAsync(cmd.QuintaId, cmd.ClienteId, ct);
        if (existente is not null)
            return ToDto(existente, esCliente: true);

        var conversacion = Conversacion.Crear(cmd.QuintaId, quinta.Nombre, cmd.ClienteId, cmd.ClienteNombre);
        await repo.AddAsync(conversacion, ct);
        return ToDto(conversacion, esCliente: true);
    }

    private static ConversacionDto ToDto(Conversacion c, bool esCliente)
    {
        var noLeidos = esCliente
            ? c.Mensajes.Count(m => m.RemitenteRol == Domain.Enums.RemitenteRol.Propietario
                && (c.UltimoLeidoPorCliente == null || m.EnviadoEn > c.UltimoLeidoPorCliente))
            : c.Mensajes.Count(m => m.RemitenteRol == Domain.Enums.RemitenteRol.Cliente
                && (c.UltimoLeidoPorPropietario == null || m.EnviadoEn > c.UltimoLeidoPorPropietario));

        return new ConversacionDto(
            c.Id, c.QuintaId, c.QuintaNombre, c.ClienteId, c.ClienteNombre,
            c.Mensajes.LastOrDefault()?.Texto,
            c.UltimoMensajeEn,
            c.Mensajes.Count,
            noLeidos);
    }
}
