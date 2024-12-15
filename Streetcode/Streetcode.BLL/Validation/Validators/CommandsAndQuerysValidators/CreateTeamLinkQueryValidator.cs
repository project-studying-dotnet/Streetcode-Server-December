using FluentValidation;
using Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Team;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateTeamLinkQueryValidator : AbstractValidator<CreateTeamLinkQuery>
    {
        public CreateTeamLinkQueryValidator()
        {
            RuleFor(x => x.teamMember)
                .NotNull().WithMessage("TeamMember cannot be null")
                .SetValidator(new TeamMemberLinkDTOValidator());
        }
    }
}
