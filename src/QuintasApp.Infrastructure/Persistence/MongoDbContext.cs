using MongoDB.Driver;
using QuintasApp.Infrastructure.Persistence.Documents;
using QuintasApp.Infrastructure.Persistence.Models;

namespace QuintasApp.Infrastructure.Persistence;

public class MongoDbContext(IMongoDatabase database)
{
    public IMongoCollection<QuintaDocument> Quintas =>
        database.GetCollection<QuintaDocument>("quintas");

    public IMongoCollection<ReservaDocument> Reservas =>
        database.GetCollection<ReservaDocument>("reservas");

    public IMongoCollection<OpinionDocument> Opiniones =>
        database.GetCollection<OpinionDocument>("opiniones");

    public IMongoCollection<AlertaDocument> AlertasDisponibilidad =>
        database.GetCollection<AlertaDocument>("alertas_disponibilidad");

    public IMongoCollection<PushTokenDocument> PushTokens =>
        database.GetCollection<PushTokenDocument>("push_tokens");

    public IMongoCollection<FechaOcupada> FechasOcupadas =>
        database.GetCollection<FechaOcupada>("fechas_ocupadas");

    public IMongoCollection<UsuarioDocument> Usuarios =>
        database.GetCollection<UsuarioDocument>("usuarios");
}
