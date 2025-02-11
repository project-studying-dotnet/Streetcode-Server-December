﻿using FluentValidation;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Fact
{
    public class CreateFactDtoValidator : AbstractValidator<CreateFactDto>
    {
        public CreateFactDtoValidator()
        {
            RuleFor(dto => dto.Title)
                .NotNull().WithMessage("Title is required.")
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(68)
                .WithMessage("Title is required and must be no more than 68 characters.");

            RuleFor(dto => dto.FactContent)
                .NotNull().WithMessage("Fact content is required.")
                .NotEmpty().WithMessage("Fact content cannot be empty.")
                .MaximumLength(600)
                .WithMessage("Fact content is required and must be no more than 600 characters.");

            RuleFor(dto => dto.StreetcodeId)
                .NotEmpty()
                .WithMessage("StreetcodeId is required.");

            RuleFor(dto => dto.Image)
                .NotNull()
                .WithMessage("Image data is required.");
        }
    }
}
