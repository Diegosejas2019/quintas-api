using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class PushTokenDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("userId")]
    public string UserId { get; set; } = default!;

    [BsonElement("token")]
    public string Token { get; set; } = default!;

    [BsonElement("platform")]
    [BsonDefaultValue("expo")]
    public string Platform { get; set; } = "expo";

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}
