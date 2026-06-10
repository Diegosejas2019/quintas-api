using FluentValidation;
using QuintasApp.Domain.Entities;

namespace QuintasApp.Application.Features.Senas.Commands.RegistrarSena;

public class RegistrarSenaValidator : AbstractValidator<RegistrarSenaCommand>
{
    public RegistrarSenaValidator()
    {
        RuleFor(x => x.ReservaId).NotEmpty();
        RuleFor(x => x.Monto).GreaterThan(0);
        RuleFor(x => x.MetodoPago).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Porcentaje)
            .InclusiveBetween(1, 100)
            .When(x => x.Tipo == TipoSena.Porcentaje)
            .WithMessage("El porcentaje debe estar entre 1 y 100.");
    }
}
