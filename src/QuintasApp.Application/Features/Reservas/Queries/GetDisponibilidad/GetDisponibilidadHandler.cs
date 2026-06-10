using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Queries.GetDisponibilidad;

public class GetDisponibilidadHandler(IReservaRepository repo) : IRequestHandler<GetDisponibilidadQuery, List<string>>
{
    public async Task<List<string>> Handle(GetDisponibilidadQuery query, CancellationToken ct)
    {
        var fechas = await repo.GetFechasOcupadasAsync(query.QuintaId, query.Mes, query.Anio, ct);
        return fechas.Select(f => f.ToString("yyyy-MM-dd")).ToList();
    }
}
