using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class ReservaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("quintaId")]
    public string QuintaId { get; set; } = default!;

    [BsonElement("quintaNombre")]
    public string QuintaNombre { get; set; } = default!;

    [BsonElement("nombreCliente")]
    public string NombreCliente { get; set; } = default!;

    [BsonElement("emailCliente")]
    public string EmailCliente { get; set; } = default!;

    [BsonElement("telefonoCliente")]
    public string TelefonoCliente { get; set; } = default!;

    // Stored as "yyyy-MM-dd" strings — safe for range queries and timezone-agnostic
    [BsonElement("fechaInicio")]
    public string FechaInicio { get; set; } = default!;

    [BsonElement("fechaFin")]
    public string FechaFin { get; set; } = default!;

    [BsonElement("precioPorDia")]
    public decimal PrecioPorDia { get; set; }

    [BsonElement("estado")]
    public string Estado { get; set; } = default!;

    [BsonElement("sena")]
    public SenaDocument? Sena { get; set; }

    [BsonElement("usuarioId")]
    public string? UsuarioId { get; set; }

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}
