using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateTimelineItemCommandValidator : AbstractValidator<CreateTimelineItemCommand>
    {
        public CreateTimelineItemCommandValidator()
        {
            RuleFor(x => x.TimelineItem)
                .NotNull().WithMessage("TimelineItem cannot be null.")
                .SetValidator(new TimelineItemCreateDTOValidator());
        }
    }
}
