using FluentValidation;
using Football.Application.DTOs;

namespace Football.Application.Validators
{
    public class PlayerDtoValidator : AbstractValidator<PlayerDto>
    {
        public PlayerDtoValidator()
        {
            RuleFor(x => x.Number)
    .GreaterThan(0)
    .WithMessage("El número de jugador debe ser mayor que 0.");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre del jugador es obligatorio.")
                .MaximumLength(50)
                .WithMessage("El nombre no puede exceder 50 caracteres.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido del jugador es obligatorio.")
                .MaximumLength(50)
                .WithMessage("El apellido no puede exceder 50 caracteres.");
        }
    }
}