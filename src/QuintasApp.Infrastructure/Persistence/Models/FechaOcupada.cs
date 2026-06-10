using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Models;

public class FechaOcupada
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("quintaId")]
    public string QuintaId { get; set; } = default!;

    [BsonElement("fecha")]
    public DateTime Fecha { get; set; }

    [BsonElement("reservaId")]
    public string ReservaId { get; set; } = default!;
}
