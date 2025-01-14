using FluentValidation;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Audio;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Media.Audio
{
    public class AudioFileBaseUpdateDtoValidator : AbstractValidator<AudioFileBaseUpdateDTO>
    {
        public AudioFileBaseUpdateDtoValidator()
        {
            Include(new AudioFileBaseCreateDTOValidator());

            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0).WithMessage("The 'Id' must be a non-negative integer");
        }
    }
}