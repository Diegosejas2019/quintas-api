using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByCliente;

public class GetConversacionesByClienteHandler(IConversacionRepository repo)
    : IRequestHandler<GetConversacionesByClienteQuery, List<ConversacionDto>>
{
    public async Task<List<ConversacionDto>> Handle(GetConversacionesByClienteQuery query, CancellationToken ct)
    {
        var conversaciones = await repo.GetByClienteAsync(query.ClienteId, ct);
        return conversaciones.Select(ToDto).ToList();
    }

    private static ConversacionDto ToDto(Conversacion c)
    {
        var noLeidos = c.Mensajes.Count(m =>
            m.RemitenteRol == RemitenteRol.Propietario
            && (c.UltimoLeidoPorCliente == null || m.EnviadoEn > c.UltimoLeidoPorCliente));

        return new ConversacionDto(
            c.Id, c.QuintaId, c.QuintaNombre, c.ClienteId, c.ClienteNombre,
            c.Mensajes.LastOrDefault()?.Texto,
            c.UltimoMensajeEn,
            c.Mensajes.Count,
            noLeidos,
            c.UltimoLeidoPorPropietario,
            c.UltimoLeidoPorCliente);
    }
}
