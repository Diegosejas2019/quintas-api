using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuintasApp.Infrastructure.Persistence.Documents;

public class ConversacionDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = default!;

    [BsonElement("quintaId")]
    public string QuintaId { get; set; } = default!;

    [BsonElement("quintaNombre")]
    public string QuintaNombre { get; set; } = default!;

    [BsonElement("clienteId")]
    public string ClienteId { get; set; } = default!;

    [BsonElement("clienteNombre")]
    public string ClienteNombre { get; set; } = default!;

    [BsonElement("mensajes")]
    public List<MensajeDocument> Mensajes { get; set; } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("ultimoMensajeEn")]
    public DateTime UltimoMensajeEn { get; set; }

    [BsonElement("ultimoLeidoPorPropietario")]
    public DateTime? UltimoLeidoPorPropietario { get; set; }

    [BsonElement("ultimoLeidoPorCliente")]
    public DateTime? UltimoLeidoPorCliente { get; set; }
}

public class MensajeDocument
{
    [BsonRepresentation(BsonType.String)]
    [BsonElement("id")]
    public string Id { get; set; } = default!;

    [BsonElement("texto")]
    public string Texto { get; set; } = default!;

    [BsonElement("remitenteRol")]
    public string RemitenteRol { get; set; } = default!;

    [BsonElement("remitenteId")]
    public string RemitenteId { get; set; } = default!;

    [BsonElement("enviadoEn")]
    public DateTime EnviadoEn { get; set; }
}
