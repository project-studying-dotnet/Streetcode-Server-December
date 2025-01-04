using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.MediatR.Email;
using Streetcode.BLL.Validation.Validators.CommandsValidators;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class SendEmailCommandValidatorTests
    {
        private readonly SendEmailCommandValidator _validator;

        public SendEmailCommandValidatorTests()
        {
            _validator = new SendEmailCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenTeamLinkNull()
        {
            // Arrange
            var command = new SendEmailCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email object is required");
        }

        [Fact]
        public void Should_NotHaveErrorForValidEmail()
        {
            // Arrange
            var validEmail = new EmailDto
            {
                From = "test@example.com",
                Content = "Valid content"
            };

            var validCommand = new SendEmailCommand(validEmail);

            // Act
            var result = _validator.TestValidate(validCommand);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_InvokeEmailDTOValidatorForIncorrectEmail()
        {
            // Arrange
            var invalidEmail = new EmailDto
            {
                From = "test@example.com",
                Content = string.Empty
            };

            var invalidCommand = new SendEmailCommand(invalidEmail);

            // Act
            var result = _validator.TestValidate(invalidCommand);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email.Content)
                .WithErrorMessage("The 'Content' field must be at least 1 character long");
        }
    }

}
