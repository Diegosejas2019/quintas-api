using FluentValidation;

namespace QuintasApp.Application.Features.Alertas.Commands.CrearAlerta;

public class CrearAlertaValidator : AbstractValidator<CrearAlertaCommand>
{
    public CrearAlertaValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.QuintaId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.FechaFin).GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de fin debe ser posterior o igual a la fecha de inicio.");
    }
}
