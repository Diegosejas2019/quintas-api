using FluentValidation;

namespace QuintasApp.Application.Features.Reservas.Commands.CreateReserva;

public class CreateReservaValidator : AbstractValidator<CreateReservaCommand>
{
    public CreateReservaValidator()
    {
        RuleFor(x => x.QuintaId).NotEmpty();
        RuleFor(x => x.NombreCliente).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EmailCliente).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.TelefonoCliente).NotEmpty().MaximumLength(50);
        RuleFor(x => x.FechaInicio).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("La fecha de inicio no puede ser en el pasado.");
        RuleFor(x => x.FechaFin).GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de fin no puede ser anterior a la de inicio.");
    }
}
