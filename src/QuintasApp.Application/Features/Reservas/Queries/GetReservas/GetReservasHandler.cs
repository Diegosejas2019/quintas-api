using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Queries.GetReservas;

public class GetReservasHandler(IReservaRepository repo, IQuintaRepository quintaRepo)
    : IRequestHandler<GetReservasQuery, List<ReservaDto>>
{
    public async Task<List<ReservaDto>> Handle(GetReservasQuery query, CancellationToken ct)
    {
        IEnumerable<string>? quintaIds = null;
        if (query.PropietarioId is not null)
        {
            var quintas = await quintaRepo.GetAllByPropietarioAsync(query.PropietarioId, ct);
            quintaIds = quintas.Select(q => q.Id.ToString());
        }

        var reservas = await repo.GetAllAsync(query.Estado, query.QuintaId, query.Page, query.Size, ct, quintaIds);
        return reservas.Select(r => new ReservaDto(
            r.Id, r.QuintaId, r.Quinta.Nombre,
            r.NombreCliente, r.EmailCliente, r.TelefonoCliente,
            r.FechaInicio, r.FechaFin, r.CantidadDias, r.PrecioPorDia, r.PrecioTotal,
            r.Estado.ToString(),
            r.Sena == null ? null : new SenaDto(r.Sena.Id, r.Sena.Monto, r.Sena.Tipo.ToString(), r.Sena.Porcentaje, r.Sena.FechaPago, r.Sena.MetodoPago),
            r.CreatedAt
        )).ToList();
    }
}
