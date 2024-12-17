﻿using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentValidation.TestHelper;

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
        public void Should_Have_Error_When_TimelineItem_Is_Null()
        {
            var model = new CreateTimelineItemCommand(null); // Pass null explicitly
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TimelineItem);
        }

        [Fact]
        public void Should_Have_Error_When_TimelineItem_Has_Invalid_Title()
        {
            var timelineItemDto = new TimelineItemCreateDto
            {
                Date = new DateTime(2024, 1, 1),
                Title = new string('A', 27), // Exceeds 26 characters
                Description = "Valid description",
                Context = "Valid context"
            };

            var model = new CreateTimelineItemCommand(timelineItemDto); // Pass the required DTO
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor("TimelineItem.Title");
        }

        [Fact]
        public void Should_Not_Have_Error_For_Valid_TimelineItem()
        {
            var timelineItemDto = new TimelineItemCreateDto
            {
                Date = new DateTime(2024, 1, 1),
                Title = "Valid Title",
                Description = "Valid description within 400 characters.",
                Context = "Valid context within 50 characters."
            };

            var model = new CreateTimelineItemCommand(timelineItemDto); // Pass the valid DTO
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
