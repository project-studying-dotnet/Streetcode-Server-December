using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Streetcode.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateTimelineItemCommandValidatorTests
    {
        private readonly CreateTimelineItemCommandValidator _validator;

        public CreateTimelineItemCommandValidatorTests()
        {
            _validator = new CreateTimelineItemCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenTimelineItemCreateDto_IsNull()
        {
            var command = new CreateTimelineItemCommand(null);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto)
                   .WithErrorMessage("TimelineItemCreateDto cannot be null.");
        }

        [Fact]
        public void Should_HaveErrorWhenTitle_IsEmpty()
        {
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = "",
                Description = "Some description",
                Date = DateTime.Now,
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 1
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto.Title);
        }

        [Fact]
        public void Should_HaveErrorWhenTitle_IsTooLong()
        {
            var longTitle = new string('a', 27);
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = longTitle,
                Description = "Some description",
                Date = DateTime.Now,
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 1
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto.Title)
                   .WithErrorMessage("Title is required and must be no more than 26 characters.");
        }

        [Fact]
        public void Should_HaveErrorWhenDescription_IsTooLong()
        {
            var longDescription = new string('a', 401);
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = "Valid Title",
                Description = longDescription,
                Date = DateTime.Now,
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 1
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto.Description)
                   .WithErrorMessage("Description must be no more than 400 characters.");
        }

        [Fact]
        public void Should_HaveErrorWhenDate_IsDefault()
        {
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = "Valid Title",
                Description = "Some description",
                Date = default(DateTime), // Use default(DateTime) for a default DateTime value
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 1
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto.Date)
                   .WithErrorMessage("Date is required.");
        }

        [Fact]
        public void Should_HaveErrorWhenDateViewPattern_IsInvalidEnum()
        {
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = "Valid Title",
                Description = "Some description",
                Date = DateTime.Now,
                DateViewPattern = (DateViewPattern)100, // Invalid enum value
                StreetcodeId = 1
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto.DateViewPattern)
                   .WithErrorMessage("DateViewPattern is required and must be a valid enum value.");
        }

        [Fact]
        public void Should_HaveErrorWhenStreetcodeId_IsEmpty()
        {
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = "Valid Title",
                Description = "Some description",
                Date = DateTime.Now,
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 0 // Default int value
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.timelineItemCreateDto.StreetcodeId)
                   .WithErrorMessage("StreetcodeId is required.");
        }

        [Fact]
        public void Should_NotHaveErrorForValidCommand()
        {
            var command = new CreateTimelineItemCommand(new TimelineItemCreateDto
            {
                Title = "Valid Title",
                Description = "Some description",
                Date = DateTime.Now,
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 1
            });

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
