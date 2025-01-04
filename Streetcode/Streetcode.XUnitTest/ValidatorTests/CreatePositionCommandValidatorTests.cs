using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreatePositionCommandValidatorTests
    {
        private readonly CreatePositionQueryValidator _validator;

        public CreatePositionCommandValidatorTests()
        {
            _validator = new CreatePositionQueryValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenPositionNull()
        {
            // Arrange
            var command = new CreatePositionCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.position)
                  .WithErrorMessage("Position cannot be null");
        }

        [Fact]
        public void Should_NotHaveErrorForValidPosition()
        {
            // Arrange
            var position = new PositionDto
            {
                Position = "Manager",
                Id = 1
            };

            var validQuery = new CreatePositionCommand(position);

            // Act
            var result = _validator.TestValidate(validQuery);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_InvokePositionDTOValidatorForIncorrectPosition()
        {
            // Arrange
            var position = new PositionDto
            {
                Position = "Manager",
                Id = -67
            };

            var invalidQuery = new CreatePositionCommand(position);

            // Act
            var result = _validator.TestValidate(invalidQuery);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.position.Id)
                .WithErrorMessage("Id must be greater than 0");
        }
    }
}
