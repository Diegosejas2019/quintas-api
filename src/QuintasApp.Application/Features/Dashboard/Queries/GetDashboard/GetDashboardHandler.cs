using MediatR;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Dashboard.Queries.GetDashboard;

public class GetDashboardHandler(IQuintaRepository quintaRepo, IReservaRepository reservaRepo)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public async Task<DashboardDto> Handle(GetDashboardQuery query, CancellationToken ct)
    {
        var hoy = DateTime.UtcNow;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        if (query.PropietarioId is not null)
        {
            var quintas = await quintaRepo.GetAllByPropietarioAsync(query.PropietarioId, ct);
            var quintaIds = quintas.Select(q => q.Id.ToString()).ToList();
            var stats = await reservaRepo.GetDashboardStatsAsync(quintaIds, inicioMes, ct);

            return BuildDto(quintas.Count, stats);
        }
        else
        {
            var quintasAllTask = quintaRepo.GetAllAsync(ct);
            var globalStatsTask = reservaRepo.GetDashboardStatsAsync([], inicioMes, ct);
            await Task.WhenAll(quintasAllTask, globalStatsTask);

            return BuildDto(quintasAllTask.Result.Count, globalStatsTask.Result);
        }
    }

    private static DashboardDto BuildDto(
        int totalQuintas,
        List<(string Estado, int Count, decimal IngresosTotales, decimal IngresosEsteMes)> stats)
    {
        var pendientes  = stats.FirstOrDefault(s => s.Estado == "Pendiente").Count;
        var confirmadas = stats.FirstOrDefault(s => s.Estado == "Confirmada").Count;
        var finalizadas = stats.FirstOrDefault(s => s.Estado == "Finalizada").Count;
        var ingresosTotales = stats
            .Where(s => s.Estado is "Confirmada" or "Finalizada")
            .Sum(s => s.IngresosTotales);
        var ingresosEsteMes = stats
            .Where(s => s.Estado is "Confirmada" or "Finalizada")
            .Sum(s => s.IngresosEsteMes);

        return new DashboardDto(totalQuintas, pendientes, confirmadas, finalizadas, ingresosTotales, ingresosEsteMes);
    }
}
