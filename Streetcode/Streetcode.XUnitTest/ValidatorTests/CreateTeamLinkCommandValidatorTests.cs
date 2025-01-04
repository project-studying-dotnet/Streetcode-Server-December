using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.MediatR.Team.Create;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests
{
    public class CreateTeamLinkCommandValidatorTests
    {
        private readonly CreateTeamLinkQueryValidator _validator;

        public CreateTeamLinkCommandValidatorTests()
        {
            _validator = new CreateTeamLinkQueryValidator();
        }

        [Fact]
        public void Should_HaveErrorWhenTeamLinkNull()
        {
            // Arrange
            var command = new CreateTeamLinkCommand(null);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.teamMember)
                  .WithErrorMessage("TeamMember cannot be null");
        }

        [Fact]
        public void Should_NotHaveErrorForValidTeamMemberLink()
        {
            // Arrange
            var validMember = new TeamMemberLinkDto()
            {
                Id = 1,
                TeamMemberId = 3,
                TargetUrl = "https://valid-url.com",
                LogoType = LogoTypeDto.Facebook
            };

            var validQuery = new CreateTeamLinkCommand(validMember);

            // Act
            var result = _validator.TestValidate(validQuery);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_InvokeTeamLinkMemberDTOValidatorForIncorrectTeamMember()
        {
            // Arrange
            var invalidMember = new TeamMemberLinkDto()
            {
                Id = -2,
                TeamMemberId = -3,
                TargetUrl = "invalidUrl",
                LogoType = LogoTypeDto.Facebook
            };

            var invalidQuery = new CreateTeamLinkCommand(invalidMember);

            // Act
            var result = _validator.TestValidate(invalidQuery);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.teamMember.Id)
                .WithErrorMessage("Id must be greater than 0");

            result.ShouldHaveValidationErrorFor(x => x.teamMember.TeamMemberId)
              .WithErrorMessage("TeamMemberId must be greater than 0");

            result.ShouldHaveValidationErrorFor(x => x.teamMember.TargetUrl)
              .WithErrorMessage("TargetUrl must be a valid URL");
        }
    }
}
