using MediatR;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Conversaciones.Queries.GetConversacionesByQuinta;

public class GetConversacionesByQuintaHandler(
    IConversacionRepository repo,
    IQuintaRepository quintas) : IRequestHandler<GetConversacionesByQuintaQuery, List<ConversacionDto>>
{
    public async Task<List<ConversacionDto>> Handle(GetConversacionesByQuintaQuery query, CancellationToken ct)
    {
        var quinta = await quintas.GetByIdAsync(query.QuintaId, ct)
            ?? throw new DomainException($"Quinta {query.QuintaId} no encontrada.");

        if (quinta.PropietarioId != query.PropietarioId)
            throw new UnauthorizedAccessException("No tienes acceso a las conversaciones de esta quinta.");

        var conversaciones = await repo.GetByQuintaAsync(query.QuintaId, ct);
        return conversaciones.Select(c => ToDto(c)).ToList();
    }

    private static ConversacionDto ToDto(Conversacion c)
    {
        var noLeidos = c.Mensajes.Count(m =>
            m.RemitenteRol == RemitenteRol.Cliente
            && (c.UltimoLeidoPorPropietario == null || m.EnviadoEn > c.UltimoLeidoPorPropietario));

        return new ConversacionDto(
            c.Id, c.QuintaId, c.QuintaNombre, c.ClienteId, c.ClienteNombre,
            c.Mensajes.LastOrDefault()?.Texto,
            c.UltimoMensajeEn,
            c.Mensajes.Count,
            noLeidos);
    }
}
