using FluentValidation;

namespace QuintasApp.Application.Features.Conversaciones.Commands.EnviarMensaje;

public class EnviarMensajeValidator : AbstractValidator<EnviarMensajeCommand>
{
    public EnviarMensajeValidator()
    {
        RuleFor(x => x.Texto)
            .NotEmpty().WithMessage("El texto del mensaje no puede estar vacío.")
            .MaximumLength(1000).WithMessage("El mensaje no puede superar los 1000 caracteres.");
    }
}
