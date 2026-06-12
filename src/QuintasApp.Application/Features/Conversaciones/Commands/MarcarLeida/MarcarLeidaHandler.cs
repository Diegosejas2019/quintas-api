using MediatR;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Conversaciones.Commands.MarcarLeida;

public class MarcarLeidaHandler(
    IConversacionRepository repo,
    IQuintaRepository quintas) : IRequestHandler<MarcarLeidaCommand>
{
    public async Task Handle(MarcarLeidaCommand cmd, CancellationToken ct)
    {
        var conversacion = await repo.GetByIdAsync(cmd.ConversacionId, ct)
            ?? throw new DomainException($"Conversación {cmd.ConversacionId} no encontrada.");

        var quinta = await quintas.GetByIdAsync(conversacion.QuintaId, ct)
            ?? throw new DomainException("Quinta no encontrada.");

        var esCliente = cmd.SolicitanteId == conversacion.ClienteId && cmd.SolicitanteRol == RemitenteRol.Cliente;
        var esPropietario = cmd.SolicitanteId == quinta.PropietarioId && cmd.SolicitanteRol == RemitenteRol.Propietario;

        if (!esCliente && !esPropietario)
            throw new UnauthorizedAccessException("No tienes acceso a esta conversación.");

        if (esCliente)
            conversacion.MarcarLeidaPorCliente();
        else
            conversacion.MarcarLeidaPorPropietario();

        await repo.UpdateAsync(conversacion, ct);
    }
}
