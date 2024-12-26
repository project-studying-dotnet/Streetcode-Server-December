﻿using FluentValidation;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.TextContent
{
    public class CreateFactDTOValidator : AbstractValidator<CreateFactDTO>
    {
        public CreateFactDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is mandatory.")
                .MaximumLength(68).WithMessage("Title must not exceed 68 characters.");

            RuleFor(x => x.FactContent)
                .NotEmpty().WithMessage("FactContent is mandatory.")
                .MaximumLength(600).WithMessage("FactContent must not exceed 600 characters.");
        }
    }
}
