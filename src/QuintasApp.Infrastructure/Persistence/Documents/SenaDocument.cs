using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class SenaDocument
{
    [BsonElement("id")]
    public string Id { get; set; } = default!;

    [BsonElement("monto")]
    public decimal Monto { get; set; }

    [BsonElement("tipo")]
    public string Tipo { get; set; } = default!;

    [BsonElement("porcentaje")]
    public decimal? Porcentaje { get; set; }

    [BsonElement("fechaPago")]
    public string FechaPago { get; set; } = default!;

    [BsonElement("metodoPago")]
    public string MetodoPago { get; set; } = default!;

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}
