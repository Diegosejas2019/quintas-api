using MediatR;

namespace QuintasApp.Application.Features.Quintas.Queries.GetQuintas;

public record QuintaDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    decimal PrecioPorDia,
    int Capacidad,
    List<string> Imagenes,
    bool Activa,
    string? Direccion = null,
    decimal? Latitud = null,
    decimal? Longitud = null,
    bool Pileta = false,
    bool Parrilla = false,
    List<string>? Amenities = null
);

public record GetQuintasQuery : IRequest<List<QuintaDto>>;
