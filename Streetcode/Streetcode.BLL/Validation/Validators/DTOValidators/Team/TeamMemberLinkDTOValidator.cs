using FluentValidation;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Team
{
    public class TeamMemberLinkDTOValidator : AbstractValidator<TeamMemberLinkDTO>
    {
        public TeamMemberLinkDTOValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0");

            RuleFor(x => x.TeamMemberId)
                .GreaterThan(0).WithMessage("TeamMemberId must be greater than 0");

            RuleFor(x => x.TargetUrl)
                .NotEmpty().WithMessage("TargetUrl cannot be empty")
                .Matches(@"^(http|https):\/\/[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(\/\S*)?$")
                .WithMessage("TargetUrl must be a valid URL");

            RuleFor(x => x.LogoType)
                .Must(logoType => Enum.IsDefined(typeof(LogoTypeDTO), logoType))
                .WithMessage("LogoType must be a valid enum value");
        }
    }
}
