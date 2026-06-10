using MediatR;

namespace QuintasApp.Application.Features.Dashboard.Queries.GetDashboard;

public record DashboardDto(
    int TotalQuintas,
    int ReservasPendientes,
    int ReservasConfirmadas,
    int ReservasFinalizadas,
    decimal IngresosTotales,
    decimal IngresosEsteMes
);

public record GetDashboardQuery : IRequest<DashboardDto>;
