using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class AlertaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("userId")]
    public string UserId { get; set; } = default!;

    [BsonElement("quintaId")]
    public string QuintaId { get; set; } = default!;

    [BsonElement("fechaInicio")]
    public string FechaInicio { get; set; } = default!;

    [BsonElement("fechaFin")]
    public string FechaFin { get; set; } = default!;

    [BsonElement("email")]
    public string Email { get; set; } = default!;

    [BsonElement("notificado")]
    public bool Notificado { get; set; }

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}
