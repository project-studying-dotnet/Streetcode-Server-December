using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Comment.CreateReply;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Xunit;
using FluentValidation.TestHelper;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateReplyCommandValidatorTests
    {
        private readonly CreateReplyCommandValidator _validator;

        public CreateReplyCommandValidatorTests()
        {
            _validator = new CreateReplyCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_CreateReplyDto_IsNull()
        {
            // Arrange
            var command = new CreateReplyCommand(null!);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.createReplyDto)
                .WithErrorMessage("CreateReplyDto object is required");
        }

        [Fact]
        public void Should_NotHaveError_When_CreateReplyDto_IsValid()
        {
            // Arrange
            var createReplyDto = new CreateReplyDto();

            var command = new CreateReplyCommand(createReplyDto);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.createReplyDto);
        }
    }
}
