using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System;
using System.Collections.Generic;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateImageCommandValidatorTests
    {
        private readonly CreateImageCommandValidator _validator;

        public CreateImageCommandValidatorTests()
        {
            _validator = new CreateImageCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenImageNull()
        {
            // Arrange
            var command = new CreateImageCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Image)
                  .WithErrorMessage("Image object is required");
        }

        [Fact]
        public void Should_InvokeImageFileBaseCreateDTOValidatorForIncorrectImage()
        {
            // Arrange
            var image = new ImageFileBaseCreateDTO
            {
                Alt = "Test Alt",
                Title = "Valid Title",
                MimeType = "png",
                BaseFormat = "image",
                Extension = ".png"
            };
            var command = new CreateImageCommand(image);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Image);
        }

        [Fact]
        public void Should_NotHaveErrorForValidImage()
        {
            // Arrange
            var image = new ImageFileBaseCreateDTO
            {
                Alt = "Valid Alt Text",
                Title = "Valid Title",
                MimeType = "jpeg",
                BaseFormat = "image",
                Extension = ".jpg"
            };
            var command = new CreateImageCommand(image);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

