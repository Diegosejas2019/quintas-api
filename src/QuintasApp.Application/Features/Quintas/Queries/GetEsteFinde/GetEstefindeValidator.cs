using FluentValidation;

namespace QuintasApp.Application.Features.Quintas.Queries.GetEsteFinde;

public class GetEstefindeValidator : AbstractValidator<GetEstefindeQuery>
{
    public GetEstefindeValidator()
    {
        When(q => q.FechaInicio.HasValue || q.FechaFin.HasValue, () =>
        {
            RuleFor(q => q.FechaInicio)
                .NotNull().WithMessage("FechaInicio es requerida cuando se especifica FechaFin.")
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("FechaInicio no puede ser anterior a hoy.");

            RuleFor(q => q.FechaFin)
                .NotNull().WithMessage("FechaFin es requerida cuando se especifica FechaInicio.")
                .GreaterThan(q => q.FechaInicio!.Value)
                .WithMessage("FechaFin debe ser posterior a FechaInicio.");
        });
    }
}
