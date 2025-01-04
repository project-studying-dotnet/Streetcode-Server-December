using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.Validation.Validators.Commands;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateVideoCommandValidatorTests
    {
        private readonly CreateVideoCommandValidator _validator;

        public CreateVideoCommandValidatorTests()
        {
            _validator = new CreateVideoCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenVideoNull()
        {
            // Arrange
            var command = new CreateVideoCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Video)
                  .WithErrorMessage("Video object is required");
        }

        [Fact]
        public void Should_InvokeVideoCreateDTOValidatorForIncorrectVideo()
        {
            // Arrange
            var video = new VideoCreateDto
            {
               Url = "invalid-url"
            };
            var command = new CreateVideoCommand(video);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Video.Url)
                .WithErrorMessage("Url must be a valid URL starting with http:// or https://");
        }

        [Fact]
        public void Should_NotHaveErrorsForValidModel()
        {
            // Arrange
            var video = new VideoCreateDto
            {
                StreetcodeId = 1,
                Url = "https://example.com"
            };
            var command = new CreateVideoCommand(video);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
