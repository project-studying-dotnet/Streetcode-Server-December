using FluentValidation;
using Streetcode.BLL.MediatR.Email;
using Streetcode.BLL.Validation.Validators.DTOValidators.Email;

namespace Streetcode.BLL.Validation.Validators.CommandsValidators
{
    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email object is required")
                .SetValidator(new EmailDTOValidator());
        }
    }
}
