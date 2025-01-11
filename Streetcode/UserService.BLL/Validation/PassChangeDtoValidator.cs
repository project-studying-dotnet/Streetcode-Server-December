using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Validation
{
    public class PassChangeDtoValidator : AbstractValidator<PassChangeDto>
    {
        public PassChangeDtoValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password field is required.")
                .MinimumLength(6)
                .WithMessage("Password field must be at least 6 characters long.");

            RuleFor(x => x.PasswordConfirm)
                .NotEmpty()
                .WithMessage("PasswordConfirm field is required.")
                .MinimumLength(6)
                .WithMessage("PasswordConfirm field must be at least 6 characters long.");

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("OldPassword field is required.")
                .MinimumLength(6)
                .WithMessage("OldPassword field must be at least 6 characters long.");
        }
    }
}
