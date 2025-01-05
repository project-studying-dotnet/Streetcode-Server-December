using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailService.BLL.DTO;
using FluentValidation;

namespace EmailService.BLL.Validators
{
    public class EmailDtoValidator : AbstractValidator<EmailDto>
    {
        public EmailDtoValidator()
        {
            RuleFor(x => x.ToEmail)
                .NotEmpty().WithMessage("Collection of emails cannot be null")
                .Must(emails => emails != null && emails.Any()).WithMessage("Collection must have at least one email");

            RuleForEach(x => x.ToEmail).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email cannot be null")
                .EmailAddress().WithMessage(email => $"Email '{email}' has incorrect format");

            RuleFor(x => x.FromText)
                .MaximumLength(80).WithMessage("The 'FromText' field cannot exceed 80 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("The 'Content' field is required")
                .Length(1, 10000).WithMessage("The 'Content' field must be between 1 and 10000 characters");
        }
    }
}
