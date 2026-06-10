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
    bool Parrilla = false
) : IRequest<Guid>;
