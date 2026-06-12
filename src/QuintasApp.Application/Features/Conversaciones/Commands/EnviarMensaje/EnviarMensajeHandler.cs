using MediatR;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Conversaciones.Commands.EnviarMensaje;

public class EnviarMensajeHandler(
    IConversacionRepository repo,
    IQuintaRepository quintas,
    IPushTokenRepository pushTokens,
    IPushNotificador pushNotificador) : IRequestHandler<EnviarMensajeCommand, MensajeDto>
{
    public async Task<MensajeDto> Handle(EnviarMensajeCommand cmd, CancellationToken ct)
    {
        var conversacion = await repo.GetByIdAsync(cmd.ConversacionId, ct)
            ?? throw new DomainException($"Conversación {cmd.ConversacionId} no encontrada.");

        var quinta = await quintas.GetByIdAsync(conversacion.QuintaId, ct)
            ?? throw new DomainException("Quinta no encontrada.");

        var esCliente = cmd.RemitenteId == conversacion.ClienteId && cmd.RemitenteRol == RemitenteRol.Cliente;
        var esPropietario = cmd.RemitenteId == quinta.PropietarioId && cmd.RemitenteRol == RemitenteRol.Propietario;

        if (!esCliente && !esPropietario)
            throw new UnauthorizedAccessException("No tienes acceso a esta conversación.");

        conversacion.AgregarMensaje(cmd.Texto, cmd.RemitenteRol, cmd.RemitenteId);
        await repo.UpdateAsync(conversacion, ct);

        var mensaje = conversacion.Mensajes.Last();

        if (esCliente)
        {
            var tokens = await pushTokens.GetByUserIdAsync(quinta.PropietarioId, ct);
            if (tokens.Count > 0)
            {
                await pushNotificador.EnviarAsync(
                    tokens.Select(t => t.Token),
                    $"Mensaje de {conversacion.ClienteNombre}",
                    cmd.Texto.Length > 80 ? cmd.Texto[..80] + "..." : cmd.Texto,
                    ct);
            }
        }

        return new MensajeDto(mensaje.Id, mensaje.Texto, mensaje.RemitenteRol.ToString(), mensaje.RemitenteId, mensaje.EnviadoEn);
    }
}
