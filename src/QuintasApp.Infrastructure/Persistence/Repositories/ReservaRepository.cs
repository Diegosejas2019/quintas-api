using MongoDB.Driver;
using QuintasApp.Domain.Entities;
using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence.Documents;
using QuintasApp.Infrastructure.Persistence.Models;

namespace QuintasApp.Infrastructure.Persistence.Repositories;

public class ReservaRepository(MongoDbContext db) : IReservaRepository
{
    private readonly List<Reserva> _tracked = [];

    public async Task<List<Reserva>> GetAllAsync(EstadoReserva? estado, Guid? quintaId, int page, int size, CancellationToken ct = default, IEnumerable<string>? quintaIds = null)
    {
        var filter = Builders<ReservaDocument>.Filter.Empty;
        if (estado.HasValue)
            filter &= Builders<ReservaDocument>.Filter.Eq(r => r.Estado, estado.Value.ToString());
        if (quintaId.HasValue)
            filter &= Builders<ReservaDocument>.Filter.Eq(r => r.QuintaId, quintaId.Value.ToString());
        if (quintaIds is not null)
            filter &= Builders<ReservaDocument>.Filter.In(r => r.QuintaId, quintaIds);

        var docs = await db.Reservas
            .Find(filter)
            .SortByDescending(r => r.CreatedAt)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(ct);

        return docs.Select(ToEntity).ToList();
    }

    public async Task<Reserva?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var idStr = id.ToString();
        var doc = await db.Reservas.Find(r => r.Id == idStr).FirstOrDefaultAsync(ct);
        if (doc is null) return null;
        var entity = ToEntity(doc);
        _tracked.Add(entity);
        return entity;
    }

    public async Task<List<Reserva>> GetByUsuarioIdAsync(string usuarioId, CancellationToken ct = default)
    {
        var docs = await db.Reservas
            .Find(r => r.UsuarioId == usuarioId)
            .SortByDescending(r => r.FechaInicio)
            .ToListAsync(ct);
        return docs.Select(ToEntity).ToList();
    }

    public async Task<bool> ExisteSolapamientoAsync(Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, Guid? excludeReservaId = null, CancellationToken ct = default)
    {
        var qIdStr = quintaId.ToString();
        var inicioUt = DateToUtcMidnight(fechaInicio);
        var finUt = DateToUtcMidnight(fechaFin);

        var filter = Builders<FechaOcupada>.Filter.And(
            Builders<FechaOcupada>.Filter.Eq(f => f.QuintaId, qIdStr),
            Builders<FechaOcupada>.Filter.Gte(f => f.Fecha, inicioUt),
            Builders<FechaOcupada>.Filter.Lte(f => f.Fecha, finUt));

        if (excludeReservaId.HasValue)
            filter &= Builders<FechaOcupada>.Filter.Ne(f => f.ReservaId, excludeReservaId.Value.ToString());

        return await db.FechasOcupadas.CountDocumentsAsync(filter, cancellationToken: ct) > 0;
    }

    public async Task<List<DateOnly>> GetFechasOcupadasAsync(Guid quintaId, int mes, int anio, CancellationToken ct = default)
    {
        var inicioMes = DateToUtcMidnight(new DateOnly(anio, mes, 1));
        var finMes = DateToUtcMidnight(new DateOnly(anio, mes, 1).AddMonths(1));
        var qIdStr = quintaId.ToString();

        var docs = await db.FechasOcupadas
            .Find(f => f.QuintaId == qIdStr && f.Fecha >= inicioMes && f.Fecha < finMes)
            .ToListAsync(ct);

        return docs
            .Select(f => DateOnly.FromDateTime(DateTime.SpecifyKind(f.Fecha, DateTimeKind.Utc)))
            .Distinct()
            .OrderBy(d => d)
            .ToList();
    }

    public async Task AddAsync(Reserva reserva, string quintaNombre, Guid? usuarioId = null, CancellationToken ct = default)
    {
        var doc = ToDocument(reserva, quintaNombre);
        doc.UsuarioId = usuarioId?.ToString();
        await db.Reservas.InsertOneAsync(doc, cancellationToken: ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var r in _tracked)
        {
            var idStr = r.Id.ToString();
            await db.Reservas.ReplaceOneAsync(x => x.Id == idStr, ToDocument(r, r.Quinta?.Nombre ?? ""), cancellationToken: ct);
        }
        _tracked.Clear();
    }

    public async Task InsertarFechasOcupadasAsync(Guid reservaId, Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken ct = default)
    {
        var reservaIdStr = reservaId.ToString();
        var quintaIdStr = quintaId.ToString();

        var docs = new List<FechaOcupada>();
        for (var d = fechaInicio; d <= fechaFin; d = d.AddDays(1))
        {
            docs.Add(new FechaOcupada
            {
                QuintaId = quintaIdStr,
                Fecha = DateToUtcMidnight(d),
                ReservaId = reservaIdStr
            });
        }

        try
        {
            await db.FechasOcupadas.InsertManyAsync(docs, cancellationToken: ct);
        }
        catch (MongoBulkWriteException ex) when (ex.WriteErrors.Any(e => e.Code == 11000))
        {
            // Clean up any partially inserted docs before throwing
            await db.FechasOcupadas.DeleteManyAsync(f => f.ReservaId == reservaIdStr, cancellationToken: ct);
            throw new FechasSuperposicionException(quintaIdStr, fechaInicio, fechaFin);
        }
    }

    public async Task LiberarFechasOcupadasAsync(Guid reservaId, CancellationToken ct = default)
    {
        var reservaIdStr = reservaId.ToString();
        await db.FechasOcupadas.DeleteManyAsync(f => f.ReservaId == reservaIdStr, cancellationToken: ct);
    }

    public async Task RegistrarSenaAsync(Guid reservaId, Sena sena, EstadoReserva nuevoEstado, DateTimeOffset updatedAt, CancellationToken ct = default)
    {
        var idStr = reservaId.ToString();
        var update = Builders<ReservaDocument>.Update
            .Set(r => r.Estado, nuevoEstado.ToString())
            .Set(r => r.UpdatedAt, updatedAt)
            .Set(r => r.Sena, ToSenaDocument(sena));
        await db.Reservas.UpdateOneAsync(r => r.Id == idStr, update, cancellationToken: ct);
    }

    private static DateTime DateToUtcMidnight(DateOnly d) =>
        new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc);

    private static Reserva ToEntity(ReservaDocument d)
    {
        var r = (Reserva)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Reserva));
        Set(r, "Id", Guid.Parse(d.Id));
        Set(r, "QuintaId", Guid.Parse(d.QuintaId));
        // Minimal Quinta stub so handlers/DTOs can access Quinta.Nombre
        var quintaStub = (Quinta)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Quinta));
        Set(quintaStub, "Id", Guid.Parse(d.QuintaId));
        Set(quintaStub, "Nombre", d.QuintaNombre ?? "");
        Set(r, "Quinta", quintaStub);
        Set(r, "NombreCliente", d.NombreCliente);
        Set(r, "EmailCliente", d.EmailCliente);
        Set(r, "TelefonoCliente", d.TelefonoCliente);
        Set(r, "FechaInicio", DateOnly.ParseExact(d.FechaInicio, "yyyy-MM-dd"));
        Set(r, "FechaFin", DateOnly.ParseExact(d.FechaFin, "yyyy-MM-dd"));
        Set(r, "PrecioPorDia", d.PrecioPorDia);
        Set(r, "Estado", Enum.Parse<EstadoReserva>(d.Estado));
        Set(r, "Sena", d.Sena is null ? null : ToSenaEntity(d.Sena, Guid.Parse(d.Id)));
        Set(r, "CreatedAt", d.CreatedAt);
        Set(r, "UpdatedAt", d.UpdatedAt);
        return r;
    }

    private static ReservaDocument ToDocument(Reserva r, string quintaNombre = "") => new()
    {
        Id = r.Id.ToString(),
        QuintaId = r.QuintaId.ToString(),
        QuintaNombre = quintaNombre,
        NombreCliente = r.NombreCliente,
        EmailCliente = r.EmailCliente,
        TelefonoCliente = r.TelefonoCliente,
        FechaInicio = r.FechaInicio.ToString("yyyy-MM-dd"),
        FechaFin = r.FechaFin.ToString("yyyy-MM-dd"),
        PrecioPorDia = r.PrecioPorDia,
        Estado = r.Estado.ToString(),
        Sena = r.Sena is null ? null : ToSenaDocument(r.Sena),
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
    };

    private static Sena ToSenaEntity(SenaDocument d, Guid reservaId)
    {
        var s = (Sena)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Sena));
        Set(s, "Id", Guid.Parse(d.Id));
        Set(s, "ReservaId", reservaId);
        Set(s, "Monto", d.Monto);
        Set(s, "Tipo", Enum.Parse<TipoSena>(d.Tipo));
        Set(s, "Porcentaje", d.Porcentaje);
        Set(s, "FechaPago", DateOnly.ParseExact(d.FechaPago, "yyyy-MM-dd"));
        Set(s, "MetodoPago", d.MetodoPago);
        Set(s, "CreatedAt", d.CreatedAt);
        return s;
    }

    private static SenaDocument ToSenaDocument(Sena s) => new()
    {
        Id = s.Id.ToString(),
        Monto = s.Monto,
        Tipo = s.Tipo.ToString(),
        Porcentaje = s.Porcentaje,
        FechaPago = s.FechaPago.ToString("yyyy-MM-dd"),
        MetodoPago = s.MetodoPago,
        CreatedAt = s.CreatedAt,
    };

    private static void Set<T>(object obj, string prop, T value)
    {
        var p = obj.GetType().GetProperty(prop,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        p?.SetValue(obj, value);
    }
}
