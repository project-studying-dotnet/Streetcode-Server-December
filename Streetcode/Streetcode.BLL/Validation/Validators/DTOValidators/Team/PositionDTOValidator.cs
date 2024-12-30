using FluentValidation;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Team
{
    public class PositionDTOValidator : AbstractValidator<PositionDto>
    {
        public PositionDTOValidator()
        {
            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position cannot be empty")
                .Length(2, 100).WithMessage("Position must be between 2 and 100 characters");

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0");
        }
    }
}
