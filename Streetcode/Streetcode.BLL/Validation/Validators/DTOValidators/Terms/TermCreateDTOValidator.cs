using FluentValidation;
using Streetcode.BLL.DTO.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Terms
{
    public class TermCreateDTOValidator : AbstractValidator<TermCreateDTO>
    {
        public TermCreateDTOValidator()
        {
            RuleFor(dto => dto.Title)
                .NotNull().WithMessage("Title is required.")
                .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.")
                .When(dto => !string.IsNullOrEmpty(dto.Title));

            RuleFor(dto => dto.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
