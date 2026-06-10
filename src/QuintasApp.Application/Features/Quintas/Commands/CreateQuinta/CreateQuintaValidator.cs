using FluentValidation;

namespace QuintasApp.Application.Features.Quintas.Commands.CreateQuinta;

public class CreateQuintaValidator : AbstractValidator<CreateQuintaCommand>
{
    public CreateQuintaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PrecioPorDia).GreaterThan(0);
        RuleFor(x => x.Capacidad).GreaterThan(0);
    }
}
