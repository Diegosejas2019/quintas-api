using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class UsuarioDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("supabaseId")]
    public string SupabaseId { get; set; } = default!;

    [BsonElement("email")]
    public string Email { get; set; } = default!;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = default!;

    [BsonElement("telefono")]
    public string? Telefono { get; set; }

    [BsonElement("tipoUsuario")]
    public string TipoUsuario { get; set; } = "cliente";

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    [BsonElement("favoritos")]
    public List<string> Favoritos { get; set; } = [];
}
