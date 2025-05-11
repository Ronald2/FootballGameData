using FluentValidation;
using Football.Application.DTOs;

namespace Football.Application.Validators
{
    public class TeamDtoValidator : AbstractValidator<TeamDto>
    {
        public TeamDtoValidator()
        {
            RuleFor(x => x.Tricode)
                .NotEmpty().WithMessage("El Tricode es obligatorio.")
                .Length(2, 3).WithMessage("El Tricode debe tener entre 2 y 3 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del equipo es obligatorio.")
                .MaximumLength(100);
            
            RuleFor(x => x.Coach).NotEmpty().WithMessage("El nombre del entrenador es obligatorio.")
                .MaximumLength(100);
            
        }
    }
}