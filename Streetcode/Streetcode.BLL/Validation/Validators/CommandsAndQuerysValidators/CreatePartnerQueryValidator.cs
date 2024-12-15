using FluentValidation;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Partners;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreatePartnerQueryValidator : AbstractValidator<CreatePartnerQuery>
    {
        public CreatePartnerQueryValidator()
        {
            RuleFor(x => x.newPartner)
                .NotNull().WithMessage("Partner information cannot be null")
                .SetValidator(new CreatePartnerDTOValidator());
        }
    }
}
