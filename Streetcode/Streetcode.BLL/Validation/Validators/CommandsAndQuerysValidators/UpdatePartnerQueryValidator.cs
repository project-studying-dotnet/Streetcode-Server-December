using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Partners;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class UpdatePartnerQueryValidator : AbstractValidator<CreatePartnerQuery>
    {
        public UpdatePartnerQueryValidator()
        {
            RuleFor(x => x.newPartner)
                .NotNull().WithMessage("Partner information cannot be null")
                .SetValidator(new CreatePartnerDTOValidator());
        }
    }
}
