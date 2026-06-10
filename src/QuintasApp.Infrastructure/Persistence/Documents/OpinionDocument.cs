using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class OpinionDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("quintaId")]
    public string QuintaId { get; set; } = default!;

    [BsonElement("userId")]
    public string UserId { get; set; } = default!;

    [BsonElement("nombreCliente")]
    public string NombreCliente { get; set; } = default!;

    [BsonElement("calificacion")]
    public int Calificacion { get; set; }

    [BsonElement("comentario")]
    public string? Comentario { get; set; }

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}
