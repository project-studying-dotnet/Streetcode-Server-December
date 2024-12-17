using FluentValidation;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Audio;

namespace Streetcode.BLL.Validation.Validators.Commands
{
    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
        public CreateAudioCommandValidator()
        {
            RuleFor(x => x.Audio)
                .NotNull().WithMessage("Audio object is required")
                .SetValidator(new AudioFileBaseCreateDTOValidator());
        }
    }
}
