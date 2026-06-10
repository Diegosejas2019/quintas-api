using MediatR;
using QuintasApp.Domain.Enums;

namespace QuintasApp.Application.Features.Reservas.Queries.GetReservas;

public record SenaDto(Guid Id, decimal Monto, string Tipo, decimal? Porcentaje, DateOnly FechaPago, string MetodoPago);

public record ReservaDto(
    Guid Id,
    Guid QuintaId,
    string NombreQuinta,
    string NombreCliente,
    string EmailCliente,
    string TelefonoCliente,
    DateOnly FechaInicio,
    DateOnly FechaFin,
    int CantidadDias,
    decimal PrecioPorDia,
    decimal PrecioTotal,
    string Estado,
    SenaDto? Sena,
    DateTimeOffset CreatedAt
);

public record GetReservasQuery(
    EstadoReserva? Estado,
    Guid? QuintaId,
    int Page = 1,
    int Size = 20
) : IRequest<List<ReservaDto>>;
