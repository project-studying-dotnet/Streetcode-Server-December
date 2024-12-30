using FluentValidation;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.MediatR.Email;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Email
{
    public sealed class EmailDTOValidator : AbstractValidator<EmailDto>
    {
        public EmailDTOValidator()
        {
            RuleFor(email => email.From)
               .MaximumLength(80)
               .WithMessage("The 'From' field cannot be longer than 80 characters");

            RuleFor(email => email.Content)
                .NotEmpty()
                .WithMessage("The 'Content' field is required")
                .MinimumLength(1)
                .WithMessage("The 'Content' field must be at least 1 character long")
                .MaximumLength(500)
                .WithMessage("The 'Content' field cannot be longer than 500 characters");
        }
    }
}
