using FluentValidation;
using Streetcode.BLL.DTO.Timeline.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Timeline
{
    public class TimelineItemCreateDTOValidator : AbstractValidator<TimelineItemCreateDto>
    {
        public TimelineItemCreateDTOValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is mandatory.");

            RuleFor(x => x.Title)
                .MaximumLength(26).WithMessage("Title must not exceed 26 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is mandatory.")
                .MaximumLength(400).WithMessage("Description must not exceed 400 characters.");
            RuleFor(x => x.TeamMember)
            .NotNull().WithMessage("Description is mandatory.");
        }
    }
}
