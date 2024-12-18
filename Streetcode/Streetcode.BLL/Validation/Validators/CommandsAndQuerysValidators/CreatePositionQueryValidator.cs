using FluentValidation;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Team;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreatePositionQueryValidator : AbstractValidator<CreatePositionCommand>
    {
        public CreatePositionQueryValidator()
        {
            RuleFor(x => x.position)
                .NotNull().WithMessage("Position cannot be null")
                .SetValidator(new PositionDTOValidator());
        }
    }
}
