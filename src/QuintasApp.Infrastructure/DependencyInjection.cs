using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using QuintasApp.Domain.Interfaces;
using QuintasApp.Infrastructure.Persistence;
using QuintasApp.Infrastructure.Persistence.Repositories;
using QuintasApp.Infrastructure.Services;

namespace QuintasApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"]
            ?? throw new InvalidOperationException("MongoDB:ConnectionString is required.");
        var databaseName = configuration["MongoDB:DatabaseName"]
            ?? throw new InvalidOperationException("MongoDB:DatabaseName is required.");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        services.AddSingleton<MongoDbContext>();

        services.AddHostedService<MongoIndexInitializer>();

        services.AddScoped<IQuintaRepository, QuintaRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();
        services.AddScoped<IOpinionRepository, OpinionRepository>();
        services.AddScoped<IAlertaDisponibilidadRepository, AlertaDisponibilidadRepository>();
        services.AddScoped<IPushTokenRepository, PushTokenRepository>();
        services.AddScoped<INotificacionService, NotificacionService>();
        services.AddSingleton<IBackgroundNotificador, BackgroundNotificador>();
        services.AddScoped<ResendEmailService>();
        services.AddScoped<ExpoPushService>();

        services.AddHttpClient("Resend", client =>
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["Resend:ApiKey"]}");
        });
        services.AddHttpClient("ExpoPush");

        return services;
    }
}
