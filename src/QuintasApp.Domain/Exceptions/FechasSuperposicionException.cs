namespace QuintasApp.Domain.Exceptions;

public class FechasSuperposicionException : DomainException
{
    public FechasSuperposicionException(string nombreQuinta, DateOnly fechaInicio, DateOnly fechaFin)
        : base($"La quinta '{nombreQuinta}' ya tiene una reserva entre el {fechaInicio:dd/MM/yyyy} y el {fechaFin:dd/MM/yyyy}.")
    { }
}
