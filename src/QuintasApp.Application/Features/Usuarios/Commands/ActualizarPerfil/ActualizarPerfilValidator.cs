using FluentValidation;

namespace QuintasApp.Application.Features.Usuarios.Commands.ActualizarPerfil;

public class ActualizarPerfilValidator : AbstractValidator<ActualizarPerfilCommand>
{
    public ActualizarPerfilValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Nombre is not null || x.Telefono is not null)
            .WithMessage("Se debe proporcionar al menos nombre o teléfono.");
    }
}
