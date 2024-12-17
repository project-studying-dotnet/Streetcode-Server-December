using FluentValidation;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Art.Image
{
    public class ImageFileBaseCreateDTOValidator : AbstractValidator<ImageFileBaseCreateDTO>
    {
        public ImageFileBaseCreateDTOValidator()
        {
            Include(new FileBaseCreateDTOValidator("image"));

            RuleFor(x => x.Alt)
                .NotEmpty().WithMessage("Alt text cannot be empty")
                .MaximumLength(50).WithMessage("The 'Alt' field cannot be longer than 50 characters");
        }
    }
}
