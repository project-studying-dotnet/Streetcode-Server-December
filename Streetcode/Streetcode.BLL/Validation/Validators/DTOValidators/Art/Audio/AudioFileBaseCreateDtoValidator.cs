using FluentValidation;
using Streetcode.BLL.DTO.Media.Audio;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Art.Audio
{
    public class AudioFileBaseCreateDTOValidator : AbstractValidator<AudioFileBaseCreateDTO>
    {
        public AudioFileBaseCreateDTOValidator()
        {
            Include(new FileBaseCreateDTOValidator("audio"));

            RuleFor(x => x.Description)
                .MaximumLength(400).WithMessage("The 'Description' field cannot be longer than 400 characters");
        }
    }
}
