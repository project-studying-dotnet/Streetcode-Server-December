using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.DTO.Streetcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.AdditionalContent.Tag
{
    public class StreetcodeTagDTOValidator : AbstractValidator<StreetcodeTagDTO>
    {
        public StreetcodeTagDTOValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
        }
    }
}
