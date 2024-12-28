using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Validation.Validators.DTOValidators.AdditionalContent.Tag;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Audio;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Image;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Streetcode
{
    public class StreetcodeMainPageCreateDTOValidator : AbstractValidator<StreetcodeMainPageCreateDTO>
    {
        public StreetcodeMainPageCreateDTOValidator()
        {
            RuleFor(x => x.Index)
                .NotEmpty().WithMessage("The '{PropertyName}' field is required");
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
            RuleFor(x => x.Teaser)
                .MaximumLength(450).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
            RuleFor(x => x.EventStartOrPersonBirthDate)
                .NotEmpty().WithMessage("The '{PropertyName}' field is required");
            RuleFor(x => x.TransliterationUrl)
                .NotEmpty().WithMessage("The '{PropertyName}' field is required")
                .MaximumLength(100).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
            RuleFor(x => x.BriefDescription)
                .MaximumLength(33).WithMessage("The {PropertyName} cannot be longer than {MaxLength} characters");
            RuleForEach(x => x.Tags)
                .SetValidator(new StreetcodeTagDTOValidator());
            RuleFor(x => x.Images)
                .Must(m => m.Count(c => c.MimeType == "gif") <= 1);
            RuleFor(x => x.Audio)
                .SetValidator(new AudioFileBaseCreateDTOValidator());
        }
    }
}
