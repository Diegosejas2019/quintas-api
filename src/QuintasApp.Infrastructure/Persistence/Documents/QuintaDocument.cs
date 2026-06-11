using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

[BsonIgnoreExtraElements]
public class QuintaDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("nombre")]
    public string Nombre { get; set; } = default!;

    [BsonElement("descripcion")]
    public string? Descripcion { get; set; }

    [BsonElement("precioPorDia")]
    public decimal PrecioPorDia { get; set; }

    [BsonElement("capacidad")]
    public int Capacidad { get; set; }

    [BsonElement("imagenes")]
    public List<string> Imagenes { get; set; } = [];

    [BsonElement("propietarioId")]
    public string PropietarioId { get; set; } = default!;

    [BsonElement("activa")]
    public bool Activa { get; set; }

    [BsonElement("pileta")]
    public bool Pileta { get; set; }

    [BsonElement("parrilla")]
    public bool Parrilla { get; set; }

    [BsonElement("direccion")]
    public string? Direccion { get; set; }

    [BsonElement("latitud")]
    public decimal? Latitud { get; set; }

    [BsonElement("longitud")]
    public decimal? Longitud { get; set; }

    [BsonElement("amenities")]
    public List<string> Amenities { get; set; } = [];

    [BsonElement("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}
