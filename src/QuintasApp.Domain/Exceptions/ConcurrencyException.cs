namespace QuintasApp.Domain.Exceptions;

public class ConcurrencyException(string message) : Exception(message);
