using MediatR;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Dashboard.Queries.GetDashboard;

public class GetDashboardHandler(IQuintaRepository quintaRepo, IReservaRepository reservaRepo)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public async Task<DashboardDto> Handle(GetDashboardQuery query, CancellationToken ct)
    {
        var quintas = await quintaRepo.GetAllAsync(ct);
        var pendientes = await reservaRepo.GetAllAsync(EstadoReserva.Pendiente, null, 1, 1000, ct);
        var confirmadas = await reservaRepo.GetAllAsync(EstadoReserva.Confirmada, null, 1, 1000, ct);
        var finalizadas = await reservaRepo.GetAllAsync(EstadoReserva.Finalizada, null, 1, 1000, ct);

        var hoy = DateTime.UtcNow;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var todasConSeña = confirmadas.Concat(finalizadas).Where(r => r.Sena != null);
        var ingresosTotales = todasConSeña.Sum(r => r.Sena!.Monto);
        var ingresosEsteMes = todasConSeña
            .Where(r => r.CreatedAt >= inicioMes)
            .Sum(r => r.Sena!.Monto);

        return new DashboardDto(
            quintas.Count,
            pendientes.Count,
            confirmadas.Count,
            finalizadas.Count,
            ingresosTotales,
            ingresosEsteMes
        );
    }
}
