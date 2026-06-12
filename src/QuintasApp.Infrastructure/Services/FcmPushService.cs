using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using QuintasApp.Infrastructure.Persistence;
using QuintasApp.Infrastructure.Persistence.Documents;

namespace QuintasApp.Infrastructure.Services;

public class FcmPushService
{
    private readonly FirebaseMessaging _messaging;
    private readonly MongoDbContext _db;
    private readonly ILogger<FcmPushService> _logger;

    public FcmPushService(IConfiguration configuration, MongoDbContext db, ILogger<FcmPushService> logger)
    {
        _db = db;
        _logger = logger;

        var serviceAccountJson = configuration["Firebase:ServiceAccountJson"]
            ?? throw new InvalidOperationException("Firebase:ServiceAccountJson is required.");

        var credential = GoogleCredential.FromJson(serviceAccountJson);
        var app = FirebaseApp.DefaultInstance ?? FirebaseApp.Create(new AppOptions
        {
            Credential = credential
        });

        _messaging = FirebaseMessaging.GetMessaging(app);
    }

    public async Task EnviarAsync(IEnumerable<string> tokens, string titulo, string cuerpo, CancellationToken ct = default)
    {
        var tokenList = tokens.ToList();
        if (tokenList.Count == 0) return;

        var tasks = tokenList.Select(token => EnviarUnoAsync(token, titulo, cuerpo, ct));
        await Task.WhenAll(tasks);
    }

    private async Task EnviarUnoAsync(string token, string titulo, string cuerpo, CancellationToken ct)
    {
        try
        {
            await _messaging.SendAsync(new Message
            {
                Token = token,
                Notification = new Notification { Title = titulo, Body = cuerpo }
            }, ct);
        }
        catch (FirebaseMessagingException ex) when (
            ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
            ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
        {
            _logger.LogWarning("FCM token inválido {Token}, eliminando", token[..Math.Min(20, token.Length)]);
            await _db.PushTokens.DeleteOneAsync(
                Builders<PushTokenDocument>.Filter.Eq(p => p.Token, token),
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando notificación FCM a token {Token}", token[..Math.Min(20, token.Length)]);
        }
    }
}
