using FluentValidation;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.TimelineItem;
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
            RuleFor(command => command.timelineItemCreateDto)
                .NotNull()
                .WithMessage("TimelineItemCreateDto cannot be null.");

            RuleFor(command => command.timelineItemCreateDto)
                .SetValidator(new TimelineItemCreateDtoValidator());
        }
    }
}
