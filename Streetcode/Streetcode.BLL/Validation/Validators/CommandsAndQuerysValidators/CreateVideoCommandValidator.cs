using FluentValidation;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Audio;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Video;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateVideoCommandValidator : AbstractValidator<CreateVideoCommand>
    {
        public CreateVideoCommandValidator()
        {
            RuleFor(x => x.Video)
                .NotNull().WithMessage("Video object is required")
                .SetValidator(new VideoCreateDTOValidator());
        }
    }
}
