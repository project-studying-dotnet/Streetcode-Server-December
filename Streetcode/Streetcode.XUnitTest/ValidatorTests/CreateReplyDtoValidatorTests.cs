using System;
using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Validation.Validators.DTOValidators.Comment;
using Xunit;

namespace Streetcode.Tests.Validation.Validators
{
    public class CreateReplyDtoValidatorTests
    {
        private readonly CreateReplyDtoValidator _validator;

        public CreateReplyDtoValidatorTests()
        {
            _validator = new CreateReplyDtoValidator();
        }

        [Fact]
        public void Should_HaveError_When_UserNameIsEmpty()
        {
            // Arrange
            var model = new CreateReplyDto { UserName = "" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("UserName is required.");
        }

        [Fact]
        public void Should_HaveError_When_UserNameExceedsMaxLength()
        {
            // Arrange
            var model = new CreateReplyDto { UserName = new string('a', 51) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                  .WithErrorMessage("UserName must not exceed 50 characters.");
        }

        [Fact]
        public void Should_HaveError_When_UserFullNameIsEmpty()
        {
            // Arrange
            var model = new CreateReplyDto { UserFullName = "" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserFullName)
                  .WithErrorMessage("UserFullName is required.");
        }

        [Fact]
        public void Should_HaveError_When_UserFullNameExceedsMaxLength()
        {
            // Arrange
            var model = new CreateReplyDto { UserFullName = new string('a', 101) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserFullName)
                  .WithErrorMessage("UserFullName must not exceed 100 characters.");
        }

        [Fact]
        public void Should_HaveError_When_CreatedDateIsInFuture()
        {
            // Arrange
            var model = new CreateReplyDto { CreatedDate = DateTime.Now.AddMinutes(1) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CreatedDate)
                  .WithErrorMessage("CreatedDate cannot be in the future.");
        }

        [Fact]
        public void Should_HaveError_When_DateModifiedIsEarlierThanCreatedDate()
        {
            // Arrange
            var model = new CreateReplyDto { CreatedDate = DateTime.Now, DateModified = DateTime.Now.AddMinutes(-1) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DateModified)
                  .WithErrorMessage("DateModified cannot be earlier than CreatedDate.");
        }

        [Fact]
        public void Should_HaveError_When_ContentIsEmpty()
        {
            // Arrange
            var model = new CreateReplyDto { Content = "" };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content)
                  .WithErrorMessage("Content is required.");
        }

        [Fact]
        public void Should_HaveError_When_ContentExceedsMaxLength()
        {
            // Arrange
            var model = new CreateReplyDto { Content = new string('a', 501) };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Content)
                  .WithErrorMessage("Content must not exceed 500 characters.");
        }

        [Fact]
        public void Should_HaveError_When_StreetcodeIdIsZero()
        {
            // Arrange
            var model = new CreateReplyDto { StreetcodeId = 0 };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId)
                  .WithErrorMessage("StreetcodeId must be greater than 0.");
        }

        [Fact]
        public void Should_HaveError_When_ParentIdIsZero()
        {
            // Arrange
            var model = new CreateReplyDto { ParentId = 0 };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ParentId)
                  .WithErrorMessage("ParentId must be greater than 0.");
        }

        [Fact]
        public void Should_NotHaveAnyErrors_When_ModelIsValid()
        {
            var now = DateTime.UtcNow.AddSeconds(-1);

            // Arrange
            var model = new CreateReplyDto
            {
                UserName = "ValidUser",
                UserFullName = "Valid Full Name",
                CreatedDate = now,
                Content = "Valid content",
                StreetcodeId = 1,
                ParentId = 1
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
