using MediatR;

namespace QuintasApp.Application.Features.Quintas.Commands.UpdateQuinta;

public record UpdateQuintaCommand(
    Guid Id,
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
    string PropietarioId = ""
) : IRequest;
