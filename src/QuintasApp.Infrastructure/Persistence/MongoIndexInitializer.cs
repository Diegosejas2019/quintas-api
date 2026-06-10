using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using QuintasApp.Infrastructure.Persistence.Documents;
using QuintasApp.Infrastructure.Persistence.Models;

namespace QuintasApp.Infrastructure.Persistence;

public class MongoIndexInitializer(MongoDbContext db, ILogger<MongoIndexInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        try
        {
            // Unique index: (quintaId, fecha) — atomically enforces no double-booking
            var fechasKeys = Builders<FechaOcupada>.IndexKeys
                .Ascending(f => f.QuintaId)
                .Ascending(f => f.Fecha);
            await db.FechasOcupadas.Indexes.CreateOneAsync(
                new CreateIndexModel<FechaOcupada>(fechasKeys,
                    new CreateIndexOptions { Unique = true, Name = "ux_quintaId_fecha" }),
                cancellationToken: ct);

            // Index on reservaId for fast cleanup on cancellation
            var fechasReservaKey = Builders<FechaOcupada>.IndexKeys.Ascending(f => f.ReservaId);
            await db.FechasOcupadas.Indexes.CreateOneAsync(
                new CreateIndexModel<FechaOcupada>(fechasReservaKey,
                    new CreateIndexOptions { Name = "ix_fechas_reservaId" }),
                cancellationToken: ct);

            // Index on quintaId for reservation queries
            var reservasQuintaKey = Builders<ReservaDocument>.IndexKeys.Ascending(r => r.QuintaId);
            await db.Reservas.Indexes.CreateOneAsync(
                new CreateIndexModel<ReservaDocument>(reservasQuintaKey,
                    new CreateIndexOptions { Name = "ix_reservas_quintaId" }),
                cancellationToken: ct);

            // Index on userId for alerta queries
            var alertasUserKey = Builders<AlertaDocument>.IndexKeys.Ascending(a => a.UserId);
            await db.AlertasDisponibilidad.Indexes.CreateOneAsync(
                new CreateIndexModel<AlertaDocument>(alertasUserKey,
                    new CreateIndexOptions { Name = "ix_alertas_userId" }),
                cancellationToken: ct);

            // Index on quintaId in alertas for notification dispatch
            var alertasQuintaKey = Builders<AlertaDocument>.IndexKeys.Ascending(a => a.QuintaId);
            await db.AlertasDisponibilidad.Indexes.CreateOneAsync(
                new CreateIndexModel<AlertaDocument>(alertasQuintaKey,
                    new CreateIndexOptions { Name = "ix_alertas_quintaId" }),
                cancellationToken: ct);

            logger.LogInformation("MongoDB indexes initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize MongoDB indexes.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
