using MediatR;
using QuintasApp.Domain.Entities;

namespace QuintasApp.Application.Features.Senas.Commands.RegistrarSena;

public record RegistrarSenaCommand(
    Guid ReservaId,
    decimal Monto,
    TipoSena Tipo,
    decimal? Porcentaje,
    DateOnly FechaPago,
    string MetodoPago
) : IRequest<Guid>;
