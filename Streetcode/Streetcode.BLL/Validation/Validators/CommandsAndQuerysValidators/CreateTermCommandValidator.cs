using FluentValidation;
using Streetcode.BLL.MediatR.Terms; 
using Streetcode.BLL.Validation.Validators.DTOValidators.Terms;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateTermCommandValidator : AbstractValidator<CreateTermCommand>
    {
        public CreateTermCommandValidator()
        {
            RuleFor(command => command.TermCreateDTO)
                .NotNull().WithMessage("Term data cannot be null.")
                .SetValidator(new TermCreateDTOValidator());
        }
    }
}