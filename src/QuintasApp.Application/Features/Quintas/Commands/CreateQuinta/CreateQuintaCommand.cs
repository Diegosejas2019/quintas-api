using MediatR;

namespace QuintasApp.Application.Features.Quintas.Commands.CreateQuinta;

public record CreateQuintaCommand(
    string Nombre,
    string? Descripcion,
    decimal PrecioPorDia,
    int Capacidad,
    List<string>? Imagenes,
    string? Direccion = null,
    bool Pileta = false,
    bool Parrilla = false,
    List<string>? Amenities = null,
    decimal? Latitud = null,
    decimal? Longitud = null,
    string PropietarioId = "",
    string? HoraInicio = null,
    string? HoraFin = null
) : IRequest<Guid>;
