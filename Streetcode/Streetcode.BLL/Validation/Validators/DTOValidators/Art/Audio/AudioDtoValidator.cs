using FluentValidation;
using Streetcode.BLL.DTO.Media.Audio;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Media.Audio
{
    public class AudioDtoValidator : AbstractValidator<AudioDto>
    {
        public AudioDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0).WithMessage("The 'Id' must be a non-negative integer");

            RuleFor(x => x.Description)
                .MaximumLength(400).WithMessage("The 'Description' field cannot be longer than 400 characters");

            RuleFor(x => x.BlobName)
                .NotEmpty().WithMessage("The 'BlobName' field is required");

            RuleFor(x => x.Base64)
                .NotEmpty().WithMessage("The 'Base64' field is required");

            RuleFor(x => x.MimeType)
                .NotEmpty().WithMessage("The 'MimeType' field is required");
        }
    }
}
