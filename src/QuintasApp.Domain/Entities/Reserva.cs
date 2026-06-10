using QuintasApp.Domain.Enums;
using QuintasApp.Domain.Exceptions;

namespace QuintasApp.Domain.Entities;

public class Reserva
{
    public Guid Id { get; private set; }
    public Guid QuintaId { get; private set; }
    public Quinta Quinta { get; private set; } = default!;
    public string NombreCliente { get; private set; } = default!;
    public string EmailCliente { get; private set; } = default!;
    public string TelefonoCliente { get; private set; } = default!;
    public DateOnly FechaInicio { get; private set; }
    public DateOnly FechaFin { get; private set; }
    public int CantidadDias => FechaFin.DayNumber - FechaInicio.DayNumber + 1;
    public decimal PrecioPorDia { get; private set; }
    public decimal PrecioTotal => PrecioPorDia * CantidadDias;
    public EstadoReserva Estado { get; private set; }
    public Sena? Sena { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Reserva() { }

    public static Reserva Crear(
        Guid quintaId,
        string nombreCliente,
        string emailCliente,
        string telefonoCliente,
        DateOnly fechaInicio,
        DateOnly fechaFin,
        decimal precioPorDia)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        if (fechaInicio < hoy)
            throw new DomainException("La fecha de inicio no puede ser en el pasado.");
        if (fechaFin < fechaInicio)
            throw new DomainException("La fecha de fin no puede ser anterior a la de inicio.");
        if (string.IsNullOrWhiteSpace(nombreCliente))
            throw new DomainException("El nombre del cliente es requerido.");
        if (string.IsNullOrWhiteSpace(emailCliente))
            throw new DomainException("El email del cliente es requerido.");

        return new Reserva
        {
            Id = Guid.NewGuid(),
            QuintaId = quintaId,
            NombreCliente = nombreCliente.Trim(),
            EmailCliente = emailCliente.Trim().ToLowerInvariant(),
            TelefonoCliente = telefonoCliente.Trim(),
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            PrecioPorDia = precioPorDia,
            Estado = EstadoReserva.Pendiente,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Confirmar()
    {
        if (Estado != EstadoReserva.Pendiente)
            throw new DomainException($"Solo se pueden confirmar reservas en estado Pendiente. Estado actual: {Estado}.");
        Estado = EstadoReserva.Confirmada;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancelar()
    {
        if (Estado == EstadoReserva.Cancelada)
            throw new DomainException("La reserva ya está cancelada.");
        if (Estado == EstadoReserva.Finalizada)
            throw new DomainException("No se puede cancelar una reserva finalizada.");
        Estado = EstadoReserva.Cancelada;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Finalizar()
    {
        if (Estado != EstadoReserva.Confirmada)
            throw new DomainException("Solo se pueden finalizar reservas confirmadas.");
        Estado = EstadoReserva.Finalizada;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
