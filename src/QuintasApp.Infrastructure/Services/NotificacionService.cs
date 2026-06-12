using MongoDB.Driver;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Services;

public class NotificacionService(
    MongoDbContext db,
    ResendEmailService emailService,
    ExpoPushService expoPushService,
    FcmPushService fcmPushService) : INotificacionService
{
    public async Task NotificarCancelacionAsync(Guid quintaId, DateOnly fechaInicio, DateOnly fechaFin, CancellationToken ct = default)
    {
        var qIdStr = quintaId.ToString();
        var quinta = await db.Quintas.Find(q => q.Id == qIdStr).FirstOrDefaultAsync(ct);
        if (quinta is null) return;

        var inicioStr = fechaInicio.ToString("yyyy-MM-dd");
        var finStr = fechaFin.ToString("yyyy-MM-dd");

        var alertaFilter = Builders<AlertaDocument>.Filter.And(
            Builders<AlertaDocument>.Filter.Eq(a => a.QuintaId, qIdStr),
            Builders<AlertaDocument>.Filter.Eq(a => a.Notificado, false),
            Builders<AlertaDocument>.Filter.Lte(a => a.FechaInicio, finStr),
            Builders<AlertaDocument>.Filter.Gte(a => a.FechaFin, inicioStr));

        var alertas = await db.AlertasDisponibilidad.Find(alertaFilter).ToListAsync(ct);
        if (alertas.Count == 0) return;

        var userIds = alertas.Select(a => a.UserId).Distinct().ToList();

        var tokenDocs = await db.PushTokens
            .Find(Builders<PushTokenDocument>.Filter.In(p => p.UserId, userIds))
            .ToListAsync(ct);

        var expoTokens = tokenDocs.Where(t => (t.Platform ?? "expo") == "expo").Select(t => t.Token).ToList();
        var fcmTokens  = tokenDocs.Where(t => t.Platform == "fcm").Select(t => t.Token).ToList();

        var titulo = $"¡{quinta.Nombre} disponible!";
        var cuerpo = $"Fechas {fechaInicio:dd/MM} - {fechaFin:dd/MM} liberadas. ¡Reservá ahora!";

        var tasks = new List<Task>();
        tasks.AddRange(alertas.Select(alerta =>
            emailService.SendAlertaDisponibilidadAsync(alerta.Email, quinta.Nombre, fechaInicio, fechaFin, ct)));
        if (expoTokens.Count > 0) tasks.Add(expoPushService.SendAsync(expoTokens, titulo, cuerpo, ct));
        if (fcmTokens.Count > 0)  tasks.Add(fcmPushService.EnviarAsync(fcmTokens, titulo, cuerpo, ct));

        await Task.WhenAll(tasks);

        var alertaIds = alertas.Select(a => a.Id).ToList();
        await db.AlertasDisponibilidad.UpdateManyAsync(
            Builders<AlertaDocument>.Filter.In(a => a.Id, alertaIds),
            Builders<AlertaDocument>.Update.Set(a => a.Notificado, true),
            cancellationToken: ct);
    }
}
