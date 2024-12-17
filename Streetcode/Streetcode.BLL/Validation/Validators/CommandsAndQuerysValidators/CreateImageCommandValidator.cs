using FluentValidation;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Audio;
using Streetcode.BLL.Validation.Validators.DTOValidators.Art.Image;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
    {
        public CreateImageCommandValidator()
        {
            RuleFor(x => x.Image)
                .NotNull().WithMessage("Inage object is required")
                .SetValidator(new ImageFileBaseCreateDTOValidator());
        }
    }
}
