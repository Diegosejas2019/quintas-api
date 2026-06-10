using FluentValidation;

namespace QuintasApp.Application.Features.Opiniones.Commands.CrearOpinion;

public class CrearOpinionValidator : AbstractValidator<CrearOpinionCommand>
{
    public CrearOpinionValidator()
    {
        RuleFor(x => x.QuintaId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NombreCliente).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Calificacion).InclusiveBetween(1, 5);
        RuleFor(x => x.Comentario).MaximumLength(2000).When(x => x.Comentario != null);
    }
}
