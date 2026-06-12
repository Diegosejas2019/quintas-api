using MediatR;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Conversaciones.Queries.GetMensajes;

public class GetMensajesHandler(
    IConversacionRepository repo,
    IQuintaRepository quintas) : IRequestHandler<GetMensajesQuery, List<MensajeDto>>
{
    public async Task<List<MensajeDto>> Handle(GetMensajesQuery query, CancellationToken ct)
    {
        var conversacion = await repo.GetByIdAsync(query.ConversacionId, ct)
            ?? throw new DomainException($"Conversación {query.ConversacionId} no encontrada.");

        var quinta = await quintas.GetByIdAsync(conversacion.QuintaId, ct)
            ?? throw new DomainException("Quinta no encontrada.");

        var esCliente = query.SolicitanteId == conversacion.ClienteId && query.SolicitanteRol == RemitenteRol.Cliente;
        var esPropietario = query.SolicitanteId == quinta.PropietarioId && query.SolicitanteRol == RemitenteRol.Propietario;

        if (!esCliente && !esPropietario)
            throw new UnauthorizedAccessException("No tienes acceso a esta conversación.");

        return conversacion.Mensajes
            .OrderBy(m => m.EnviadoEn)
            .Select(m => new MensajeDto(m.Id, m.Texto, m.RemitenteRol.ToString(), m.RemitenteId, m.EnviadoEn))
            .ToList();
    }
}
