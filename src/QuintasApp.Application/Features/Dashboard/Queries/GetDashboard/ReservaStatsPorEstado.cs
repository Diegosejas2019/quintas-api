namespace QuintasApp.Application.Features.Dashboard.Queries.GetDashboard;

public record ReservaStatsPorEstado(string Estado, int Count, decimal IngresosTotales, decimal IngresosEsteMes);
