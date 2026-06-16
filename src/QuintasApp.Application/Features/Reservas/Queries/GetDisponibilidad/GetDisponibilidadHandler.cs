using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Reservas.Queries.GetDisponibilidad;

public class GetDisponibilidadHandler(IReservaRepository reservaRepo, IQuintaRepository quintaRepo) : IRequestHandler<GetDisponibilidadQuery, List<string>>
{
    public async Task<List<string>> Handle(GetDisponibilidadQuery query, CancellationToken ct)
    {
        var fechasReservas = await reservaRepo.GetFechasOcupadasAsync(query.QuintaId, query.Mes, query.Anio, ct);
        var quinta = await quintaRepo.GetByIdAsync(query.QuintaId, ct);
        var fechasBloqueadas = quinta?.FechasBloqueadas
            .Where(f => f.Year == query.Anio && f.Month == query.Mes)
            .ToList() ?? [];

        return fechasReservas
            .Union(fechasBloqueadas)
            .OrderBy(f => f)
            .Select(f => f.ToString("yyyy-MM-dd"))
            .ToList();
    }
}
