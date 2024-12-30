using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Validation.Validators.Commands;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateAudioCommandValidatorTests
    {
        private readonly CreateAudioCommandValidator _validator;

        public CreateAudioCommandValidatorTests()
        {
            _validator = new CreateAudioCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenAudioNull()
        {
            // Arrange
            var command = new CreateAudioCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Audio)
                  .WithErrorMessage("Audio object is required");
        }

        [Fact]
        public void Should_InvokeAudioFileBaseCreateDTOValidatorForIncorrectAudio()
        {
            // Arrange
            var audio = new AudioFileBaseCreateDto
            {
                Description = "",
                Title = "Valid Title",
                MimeType = "mp321",
                BaseFormat = "audio",
                Extension = "_mp3"
            };
            var command = new CreateAudioCommand(audio);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Audio);
        }

        [Fact]
        public void Should_NotHaveErrorsForValidModel()
        {
            // Arrange
            var audio = new AudioFileBaseCreateDto
            {
                Description = "Test Description",
                Title = "Valid Title",
                MimeType = "mp3",
                BaseFormat = "audio",
                Extension = ".mp3"
            };
            var command = new CreateAudioCommand(audio);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
