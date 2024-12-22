﻿using FluentValidation;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Validation
{
    public class LoginDtoValidator : AbstractValidator<LoginDTO>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("UserName is required.")
                .Length(3, 50)
                .WithMessage("UserName must be between 3 and 50 characters.");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("FullName is required.")
                .Length(3, 100)
                .WithMessage("FullName must be between 3 and 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");
        }
    }
}
