using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Partners.Create;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreatePartnerQueryValidatorTests
    {
        private readonly CreatePartnerQueryValidator _validator;

        public CreatePartnerQueryValidatorTests()
        {
            _validator = new CreatePartnerQueryValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenAudioNull()
        {
            // Arrange
            var command = new CreatePartnerQuery(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.newPartner)
                  .WithErrorMessage("Partner information cannot be null");
        }

        [Fact]
        public void Should_NotHaveErrorForValidPartner()
        {
            // Arrange
            var validPartner = new CreatePartnerDTO
            {
                Id = 1,
                Title = "Valid Partner",
                Description = "A description for the valid partner.",
                TargetUrl = "https://valid-url.com",
                LogoId = 123,
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>() { new CreatePartnerSourceLinkDTO() { Id = 3, LogoType = DAL.Enums.LogoType.Twitter, TargetUrl = "https://valid-url.com" } },
                Streetcodes = new List<StreetcodeShortDTO>() { new StreetcodeShortDTO() { Id = 3, Title = "title" } }
            };

            var query = new CreatePartnerQuery(validPartner);

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_InvokePartnerDTOValidatorForIncorrectPartner()
        {
            // Arrange
            var invalidPartner = new CreatePartnerDTO
            {
                Id = -2,
                Title = "Valid Partner",
                Description = "A description for the valid partner.",
                TargetUrl = "https://valid-url.com",
                LogoId = 123,
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>() { new CreatePartnerSourceLinkDTO() { Id = 3, LogoType = DAL.Enums.LogoType.Twitter, TargetUrl = "https://valid-url.com" } },
                Streetcodes = new List<StreetcodeShortDTO>() { new StreetcodeShortDTO() { Id = 3, Title = "title" } }
            };

            var invalidQuery = new CreatePartnerQuery(invalidPartner);

            // Act
            var result = _validator.TestValidate(invalidQuery);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.newPartner.Id)
                .WithErrorMessage("Id must be greater than -1");
        }

        [Fact]
        public void Should_PartnerDTOValidatorForIncorrectPartner1()
        {
            // Arrange
            var invalidPartner = new CreatePartnerDTO
            {
                Id = -2,
                Title = "Valid Partner",
                Description = "A description for the valid partner.",
                TargetUrl = "no url",
                LogoId = 123,
                PartnerSourceLinks = new List<CreatePartnerSourceLinkDTO>(),
                Streetcodes = new List<StreetcodeShortDTO>() { new StreetcodeShortDTO() { Id = 3, Title = "title" } }
            };

            var invalidQuery = new CreatePartnerQuery(invalidPartner);

            // Act
            var result = _validator.TestValidate(invalidQuery);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.newPartner.Id)
                .WithErrorMessage("Id must be greater than -1");

            result.ShouldHaveValidationErrorFor(x => x.newPartner.TargetUrl)
                .WithErrorMessage("TargetUrl must be a valid URL starting with http or https");

            result.ShouldHaveValidationErrorFor(x => x.newPartner.PartnerSourceLinks)
                .WithErrorMessage("At least one PartnerSourceLink is required");
        }
    }
}