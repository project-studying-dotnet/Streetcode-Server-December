using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Validation
{
    public class PassResetDtoValidator : AbstractValidator<PassResetDto>
    {
        public PassResetDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email field is required.")
                .EmailAddress()
                .WithMessage("Email field should be a real Email");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password field is required.")
                .MinimumLength(6)
                .WithMessage("Password field must be at least 6 characters long.");

            RuleFor(x => x.Code)
               .NotEmpty()
               .WithMessage("Code field is required.");
        }
    }
}
