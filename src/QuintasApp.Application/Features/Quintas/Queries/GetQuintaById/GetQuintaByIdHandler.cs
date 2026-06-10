using MediatR;
using QuintasApp.Application.Features.Quintas.Queries.GetQuintas;
using QuintasApp.Domain.Interfaces;

namespace QuintasApp.Application.Features.Quintas.Queries.GetQuintaById;

public class GetQuintaByIdHandler(IQuintaRepository repo) : IRequestHandler<GetQuintaByIdQuery, QuintaDto?>
{
    public async Task<QuintaDto?> Handle(GetQuintaByIdQuery query, CancellationToken ct)
    {
        var q = await repo.GetByIdAsync(query.Id, ct);
        if (q == null) return null;
        return new QuintaDto(q.Id, q.Nombre, q.Descripcion, q.PrecioPorDia, q.Capacidad, q.Imagenes, q.Activa, q.Direccion, q.Latitud, q.Longitud, q.Pileta, q.Parrilla);
    }
}
