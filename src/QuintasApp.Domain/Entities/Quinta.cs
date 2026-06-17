using QuintasApp.Domain.Exceptions;

namespace QuintasApp.Domain.Entities;

public class Quinta
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = default!;
    public string? Descripcion { get; private set; }
    public decimal PrecioPorDia { get; private set; }
    public int Capacidad { get; private set; }
    public List<string> Imagenes { get; private set; } = [];
    public List<string> Amenities { get; private set; } = [];
    public bool Activa { get; private set; }
    public string PropietarioId { get; private set; } = default!;
    public bool Pileta { get; private set; }
    public bool Parrilla { get; private set; }
    public string? Direccion { get; private set; }
    public decimal? Latitud { get; private set; }
    public decimal? Longitud { get; private set; }
    public string? HoraInicio { get; private set; }
    public string? HoraFin { get; private set; }
    public List<DateOnly> FechasBloqueadas { get; private set; } = [];
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Quinta() { }

    public static Quinta Crear(string nombre, string? descripcion, decimal precioPorDia, int capacidad, string propietarioId, List<string>? imagenes = null, string? direccion = null, decimal? latitud = null, decimal? longitud = null, bool pileta = false, bool parrilla = false, List<string>? amenities = null, string? horaInicio = null, string? horaFin = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la quinta es requerido.");
        if (precioPorDia <= 0)
            throw new DomainException("El precio por día debe ser mayor a cero.");
        if (capacidad <= 0)
            throw new DomainException("La capacidad debe ser mayor a cero.");
        ValidarHorario(horaInicio, horaFin);

        return new Quinta
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            PrecioPorDia = precioPorDia,
            Capacidad = capacidad,
            PropietarioId = propietarioId,
            Imagenes = imagenes ?? [],
            Amenities = amenities ?? [],
            Activa = true,
            Pileta = pileta,
            Parrilla = parrilla,
            Direccion = direccion?.Trim(),
            Latitud = latitud,
            Longitud = longitud,
            HoraInicio = horaInicio,
            HoraFin = horaFin,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Actualizar(string nombre, string? descripcion, decimal precioPorDia, int capacidad, List<string>? imagenes, string? direccion = null, decimal? latitud = null, decimal? longitud = null, bool pileta = false, bool parrilla = false, List<string>? amenities = null, string? horaInicio = null, string? horaFin = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la quinta es requerido.");
        if (precioPorDia <= 0)
            throw new DomainException("El precio por día debe ser mayor a cero.");
        if (capacidad <= 0)
            throw new DomainException("La capacidad debe ser mayor a cero.");
        ValidarHorario(horaInicio, horaFin);

        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        PrecioPorDia = precioPorDia;
        Capacidad = capacidad;
        if (imagenes != null) Imagenes = imagenes;
        if (amenities != null) Amenities = amenities;
        Pileta = pileta;
        Parrilla = parrilla;
        Direccion = direccion?.Trim();
        Latitud = latitud;
        Longitud = longitud;
        HoraInicio = horaInicio;
        HoraFin = horaFin;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidarHorario(string? horaInicio, string? horaFin)
    {
        if (horaInicio is null && horaFin is null) return;
        if (horaInicio is null || horaFin is null)
            throw new DomainException("Debe especificar tanto la hora de inicio como la hora de fin.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(horaInicio, @"^([01]\d|2[0-3]):[0-5]\d$") ||
            !System.Text.RegularExpressions.Regex.IsMatch(horaFin,    @"^([01]\d|2[0-3]):[0-5]\d$"))
            throw new DomainException("El horario debe tener formato HH:mm (ej: 10:00).");
    }

    public void SetFechasBloqueadas(List<DateOnly> fechas)
    {
        FechasBloqueadas = fechas;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool TieneFechaBloqueadaEn(DateOnly inicio, DateOnly fin)
    {
        if (FechasBloqueadas.Count == 0) return false;
        var bloqueadas = new HashSet<DateOnly>(FechasBloqueadas);
        for (var d = inicio; d <= fin; d = d.AddDays(1))
            if (bloqueadas.Contains(d)) return true;
        return false;
    }

    public void Desactivar()
    {
        if (!Activa) throw new DomainException("La quinta ya está desactivada.");
        Activa = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
