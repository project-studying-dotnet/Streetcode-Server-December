using FluentValidation;
using Streetcode.BLL.DTO.Partners;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Partners
{
    public class CreatePartnerDTOValidator : AbstractValidator<CreatePartnerDto>
    {
        public CreatePartnerDTOValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(-1).WithMessage("Id must be greater than -1");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty")
                .Length(2, 200).WithMessage("Title must be between 2 and 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("The 'Description' field cannot be longer than 400 characters");

            RuleFor(x => x.TargetUrl)
                .Matches(@"^(http|https)://").WithMessage("TargetUrl must be a valid URL starting with http or https")
                .When(x => !string.IsNullOrEmpty(x.TargetUrl));

            RuleFor(x => x.UrlTitle)
                .MaximumLength(100).WithMessage("The 'UrlTitle' field cannot be longer than 400 characters")
                .When(x => !string.IsNullOrEmpty(x.UrlTitle));

            RuleFor(x => x.PartnerSourceLinks)
                .NotNull().WithMessage("PartnerSourceLinks cannot be null")
                .Must(x => x.Count > 0).WithMessage("At least one PartnerSourceLink is required")
                .When(x => x.PartnerSourceLinks != null);

            RuleFor(x => x.Streetcodes)
                .NotNull().WithMessage("Streetcodes cannot be null")
                .Must(x => x.Count > 0).WithMessage("At least one Streetcode is required");
        }
    }
}
