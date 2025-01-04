using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Analytics;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateStatisticRecordCommandValidatorTests
    {
        private readonly CreateStatisticRecordCommandValidator _validator;

        public CreateStatisticRecordCommandValidatorTests()
        {
            _validator = new CreateStatisticRecordCommandValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenStatisticRecordNull()
        {
            // Arrange
            var command = new CreateStatisticRecordCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.createStatisticRecord)
                  .WithErrorMessage("StatisticRecord cannot be null");
        }

        [Fact]
        public void Should_InvokeStatisticRecordCreateDTOValidatorForIncorrectStatisticRecord()
        {
            // Arrange
            var statisticRecord = new CreateStatisticRecordDto
            {
                QrId = 3,
                Address = "house123",
                Count = -2,
                StreetcodeId = 1111
            };
            var command = new CreateStatisticRecordCommand(statisticRecord);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.createStatisticRecord.Count)
               .WithErrorMessage("Count cannot be negative");
        }

        [Fact]
        public void Should_NotHaveErrorsForValidModel()
        {
            // Arrange
            var statisticRecord = new CreateStatisticRecordDto
            {
                QrId = 3,
                Address = "house123",
                Count = -2,
                StreetcodeId = 1111
            };
            var command = new CreateStatisticRecordCommand(statisticRecord);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.createStatisticRecord);
        }
    }
}
