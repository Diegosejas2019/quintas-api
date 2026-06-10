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
    public bool Pileta { get; private set; }
    public bool Parrilla { get; private set; }
    public string? Direccion { get; private set; }
    public decimal? Latitud { get; private set; }
    public decimal? Longitud { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Quinta() { }

    public static Quinta Crear(string nombre, string? descripcion, decimal precioPorDia, int capacidad, List<string>? imagenes = null, string? direccion = null, decimal? latitud = null, decimal? longitud = null, bool pileta = false, bool parrilla = false, List<string>? amenities = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la quinta es requerido.");
        if (precioPorDia <= 0)
            throw new DomainException("El precio por día debe ser mayor a cero.");
        if (capacidad <= 0)
            throw new DomainException("La capacidad debe ser mayor a cero.");

        return new Quinta
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim(),
            PrecioPorDia = precioPorDia,
            Capacidad = capacidad,
            Imagenes = imagenes ?? [],
            Amenities = amenities ?? [],
            Activa = true,
            Pileta = pileta,
            Parrilla = parrilla,
            Direccion = direccion?.Trim(),
            Latitud = latitud,
            Longitud = longitud,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Actualizar(string nombre, string? descripcion, decimal precioPorDia, int capacidad, List<string>? imagenes, string? direccion = null, decimal? latitud = null, decimal? longitud = null, bool pileta = false, bool parrilla = false, List<string>? amenities = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la quinta es requerido.");
        if (precioPorDia <= 0)
            throw new DomainException("El precio por día debe ser mayor a cero.");
        if (capacidad <= 0)
            throw new DomainException("La capacidad debe ser mayor a cero.");

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
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Desactivar()
    {
        if (!Activa) throw new DomainException("La quinta ya está desactivada.");
        Activa = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
