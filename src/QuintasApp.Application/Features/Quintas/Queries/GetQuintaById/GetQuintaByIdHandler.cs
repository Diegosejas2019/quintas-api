using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Queries.GetQuintaById;

public class GetQuintaByIdHandler(
    IQuintaRepository repo,
    IReservaRepository reservas,
    IUsuarioRepository usuarios) : IRequestHandler<GetQuintaByIdQuery, QuintaDto?>
{
    public async Task<QuintaDto?> Handle(GetQuintaByIdQuery query, CancellationToken ct)
    {
        var q = await repo.GetByIdAsync(query.Id, ct);
        if (q == null) return null;

        string? telefono = null;
        if (query.ClienteId is not null)
        {
            var tieneSeña = await reservas.TieneSeñaEnQuintaAsync(query.ClienteId, query.Id, ct);
            if (tieneSeña)
            {
                var propietario = await usuarios.GetBySupabaseIdAsync(q.PropietarioId, ct);
                telefono = propietario?.Telefono;
            }
        }

        return new QuintaDto(q.Id, q.Nombre, q.Descripcion, q.PrecioPorDia, q.Capacidad, q.Imagenes, q.Activa, q.Direccion, q.Latitud, q.Longitud, q.Pileta, q.Parrilla, q.Amenities, q.HoraInicio, q.HoraFin, telefono);
    }
}
