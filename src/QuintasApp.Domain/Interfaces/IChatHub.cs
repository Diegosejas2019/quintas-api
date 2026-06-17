namespace QuintasApp.Domain.Interfaces;

public interface IChatHub
{
    Task EmitirMensajeAsync(Guid conversacionId, object mensajeDto);
    Task EmitirLeidoAsync(Guid conversacionId, string quien, DateTime timestamp);
}
