using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class QuintaRepository(MongoDbContext db) : IQuintaRepository
{
    private readonly List<Quinta> _tracked = [];

    public async Task<List<Quinta>> GetAllAsync(CancellationToken ct = default)
    {
        var docs = await db.Quintas
            .Find(q => q.Activa)
            .SortBy(q => q.Nombre)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task<Quinta?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var idStr = id.ToString();
        var doc = await db.Quintas.Find(q => q.Id == idStr).FirstOrDefaultAsync(ct);
        if (doc is null) return null;
        var entity = ToEntity(doc);
        _tracked.Add(entity);
        return entity;
    }

    public async Task<List<Quinta>> GetDisponiblesEstefindeAsync(
        DateOnly viernes, DateOnly lunesExclusive,
        int? capacidad, decimal? precioMax, bool? pileta, bool? parrilla,
        CancellationToken ct = default)
    {
        var viernesStr = viernes.ToString("yyyy-MM-dd");
        var domingoStr = lunesExclusive.AddDays(-1).ToString("yyyy-MM-dd");

        var ocupadasIds = await db.Reservas
            .Find(Builders<ReservaDocument>.Filter.And(
                Builders<ReservaDocument>.Filter.Ne(r => r.Estado, EstadoReserva.Cancelada.ToString()),
                Builders<ReservaDocument>.Filter.Lte(r => r.FechaInicio, domingoStr),
                Builders<ReservaDocument>.Filter.Gte(r => r.FechaFin, viernesStr)))
            .Project(r => r.QuintaId)
            .ToListAsync(ct);

        var ocupadasSet = ocupadasIds.ToHashSet();

        var filter = Builders<QuintaDocument>.Filter.And(
            Builders<QuintaDocument>.Filter.Eq(q => q.Activa, true),
            Builders<QuintaDocument>.Filter.Nin(q => q.Id, ocupadasSet));

        if (capacidad.HasValue)
            filter &= Builders<QuintaDocument>.Filter.Gte(q => q.Capacidad, capacidad.Value);
        if (precioMax.HasValue)
            filter &= Builders<QuintaDocument>.Filter.Lte(q => q.PrecioPorDia, precioMax.Value);
        if (pileta.HasValue)
            filter &= Builders<QuintaDocument>.Filter.Eq(q => q.Pileta, pileta.Value);
        if (parrilla.HasValue)
            filter &= Builders<QuintaDocument>.Filter.Eq(q => q.Parrilla, parrilla.Value);

        var docs = await db.Quintas
            .Find(filter)
            .SortBy(q => q.PrecioPorDia)
            .ToListAsync(ct);

        return docs.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Quinta quinta, CancellationToken ct = default) =>
        await db.Quintas.InsertOneAsync(ToDocument(quinta), cancellationToken: ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var q in _tracked)
        {
            var idStr = q.Id.ToString();
            await db.Quintas.ReplaceOneAsync(x => x.Id == idStr, ToDocument(q), cancellationToken: ct);
        }
        _tracked.Clear();
    }

    private static Quinta ToEntity(QuintaDocument d)
    {
        var q = (Quinta)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Quinta));
        Set(q, "Id", Guid.Parse(d.Id));
        Set(q, "Nombre", d.Nombre);
        Set(q, "Descripcion", d.Descripcion);
        Set(q, "PrecioPorDia", d.PrecioPorDia);
        Set(q, "Capacidad", d.Capacidad);
        Set(q, "Imagenes", d.Imagenes);
        Set(q, "Activa", d.Activa);
        Set(q, "Pileta", d.Pileta);
        Set(q, "Parrilla", d.Parrilla);
        Set(q, "Amenities", d.Amenities);
        Set(q, "Direccion", d.Direccion);
        Set(q, "Latitud", d.Latitud);
        Set(q, "Longitud", d.Longitud);
        Set(q, "CreatedAt", d.CreatedAt);
        Set(q, "UpdatedAt", d.UpdatedAt);
        return q;
    }

    private static QuintaDocument ToDocument(Quinta q) => new()
    {
        Id = q.Id.ToString(),
        Nombre = q.Nombre,
        Descripcion = q.Descripcion,
        PrecioPorDia = q.PrecioPorDia,
        Capacidad = q.Capacidad,
        Imagenes = q.Imagenes,
        Activa = q.Activa,
        Pileta = q.Pileta,
        Parrilla = q.Parrilla,
        Amenities = q.Amenities,
        Direccion = q.Direccion,
        Latitud = q.Latitud,
        Longitud = q.Longitud,
        CreatedAt = q.CreatedAt,
        UpdatedAt = q.UpdatedAt,
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var p = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        p?.SetValue(obj, value);
    }
}
