using QuintasApp.Domain.Exceptions;

namespace QuintasApp.Domain.Entities;

public enum TipoSena { Fijo, Porcentaje }

public class Sena
{
    public Guid Id { get; private set; }
    public Guid ReservaId { get; private set; }
    public Reserva Reserva { get; private set; } = default!;
    public decimal Monto { get; private set; }
    public TipoSena Tipo { get; private set; }
    public decimal? Porcentaje { get; private set; }
    public DateOnly FechaPago { get; private set; }
    public string MetodoPago { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }

    private Sena() { }

    public static Sena Crear(Guid reservaId, decimal monto, TipoSena tipo, decimal? porcentaje, DateOnly fechaPago, string metodoPago, decimal precioTotal)
    {
        if (monto <= 0)
            throw new DomainException("El monto de la seña debe ser mayor a cero.");
        if (monto > precioTotal)
            throw new DomainException("La seña no puede ser mayor al precio total de la reserva.");
        if (tipo == TipoSena.Porcentaje && (porcentaje == null || porcentaje <= 0 || porcentaje > 100))
            throw new DomainException("El porcentaje debe estar entre 1 y 100.");
        if (string.IsNullOrWhiteSpace(metodoPago))
            throw new DomainException("El método de pago es requerido.");

        return new Sena
        {
            Id = Guid.NewGuid(),
            ReservaId = reservaId,
            Monto = monto,
            Tipo = tipo,
            Porcentaje = tipo == TipoSena.Porcentaje ? porcentaje : null,
            FechaPago = fechaPago,
            MetodoPago = metodoPago.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
