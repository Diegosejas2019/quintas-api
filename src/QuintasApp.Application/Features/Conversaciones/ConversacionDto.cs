namespace QuintasApp.Application.Features.Conversaciones;

public record ConversacionDto(
    Guid Id,
    Guid QuintaId,
    string QuintaNombre,
    string ClienteId,
    string ClienteNombre,
    string? UltimoMensaje,
    DateTime UltimoMensajeEn,
    int TotalMensajes,
    int MensajesNoLeidos
);

public record MensajeDto(
    Guid Id,
    string Texto,
    string RemitenteRol,
    string RemitenteId,
    DateTime EnviadoEn
);
