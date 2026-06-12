using QuintasApp.Domain.Enums;

namespace QuintasApp.Domain.Entities;

public record Mensaje(
    Guid Id,
    string Texto,
    RemitenteRol RemitenteRol,
    string RemitenteId,
    DateTime EnviadoEn);
