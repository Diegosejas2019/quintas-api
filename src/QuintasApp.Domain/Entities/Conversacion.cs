using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;

namespace QuintasApp.Domain.Entities;

public class Conversacion
{
    public Guid Id { get; private set; }
    public Guid QuintaId { get; private set; }
    public string QuintaNombre { get; private set; } = default!;
    public string ClienteId { get; private set; } = default!;
    public string ClienteNombre { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UltimoMensajeEn { get; private set; }
    public DateTime? UltimoLeidoPorPropietario { get; private set; }
    public DateTime? UltimoLeidoPorCliente { get; private set; }

    private readonly List<Mensaje> _mensajes = [];
    public IReadOnlyList<Mensaje> Mensajes => _mensajes;

    private Conversacion() { }

    public static Conversacion Crear(Guid quintaId, string quintaNombre, string clienteId, string clienteNombre)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            throw new DomainException("El clienteId es requerido.");
        if (string.IsNullOrWhiteSpace(clienteNombre))
            throw new DomainException("El nombre del cliente es requerido.");

        return new Conversacion
        {
            Id = Guid.NewGuid(),
            QuintaId = quintaId,
            QuintaNombre = quintaNombre,
            ClienteId = clienteId,
            ClienteNombre = clienteNombre,
            CreatedAt = DateTime.UtcNow,
            UltimoMensajeEn = DateTime.UtcNow
        };
    }

    public void AgregarMensaje(string texto, RemitenteRol remitenteRol, string remitenteId)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("El texto del mensaje no puede estar vacío.");
        if (texto.Length > 1000)
            throw new DomainException("El mensaje no puede superar los 1000 caracteres.");

        _mensajes.Add(new Mensaje(Guid.NewGuid(), texto.Trim(), remitenteRol, remitenteId, DateTime.UtcNow));
        UltimoMensajeEn = DateTime.UtcNow;
    }

    public void MarcarLeidaPorPropietario() => UltimoLeidoPorPropietario = DateTime.UtcNow;
    public void MarcarLeidaPorCliente() => UltimoLeidoPorCliente = DateTime.UtcNow;
}
