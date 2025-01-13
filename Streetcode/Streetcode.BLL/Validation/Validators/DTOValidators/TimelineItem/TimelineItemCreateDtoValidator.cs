using FluentValidation;
using Streetcode.BLL.DTO.Timeline.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.TimelineItem
{
    public class TimelineItemCreateDtoValidator : AbstractValidator<TimelineItemCreateDto>
    {
        public TimelineItemCreateDtoValidator()
        {
            RuleFor(dto => dto.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(26)
                .WithMessage("Title is required and must be no more than 26 characters.");

            RuleFor(dto => dto.Description)
                .MaximumLength(400)
                .WithMessage("Description must be no more than 400 characters.");

            RuleFor(dto => dto.Date)
                .NotEmpty()
                .WithMessage("Date is required.");

            RuleFor(dto => dto.DateViewPattern)
                .IsInEnum()
                .WithMessage("DateViewPattern is required and must be a valid enum value.");

            RuleFor(dto => dto.StreetcodeId)
                .NotEmpty()
                .WithMessage("StreetcodeId is required.");
        }
    }
}
