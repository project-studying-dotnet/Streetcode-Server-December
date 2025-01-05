using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateFactCommandValidatorTests
    {
        private readonly CreateFactCommandValidator _validator;

        public CreateFactCommandValidatorTests()
        {
            _validator = new CreateFactCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenFact_IsNull()
        {
            var command = new CreateFactCommand(null);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Fact)
                   .WithErrorMessage("Fact data cannot be null.");
        }

        [Fact]
        public void Should_InvokeCreateFactDtoValidator_ForValidCommand()
        {
            var command = new CreateFactCommand(new CreateFactDto
            {
                Title = "Valid Title",
                FactContent = "Some fact content",
                StreetcodeId = 1,
                Image = new CreateFactImageDto() // Assuming CreateFactImageDto exists
            });

            var result = _validator.TestValidate(command);

            // No validation errors expected on the main command level
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_PassThroughValidationErrorsFromCreateFactDtoValidator()
        {
            var command = new CreateFactCommand(new CreateFactDto
            {
                Title = "", // Empty title
                FactContent = "Some fact content",
                StreetcodeId = 1,
                Image = new CreateFactImageDto() // Assuming CreateFactImageDto exists
            });

            var result = _validator.TestValidate(command);

            // Validation error expected on the nested CreateFactDto.Title property
            result.ShouldHaveValidationErrorFor(x => x.Fact.Title)
                   .WithErrorMessage("Title is required and must be no more than 68 characters.");
        }
    }
}
