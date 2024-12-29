using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.Validation.Validators.DTOValidators.Streetcode;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateStreetcodeMainPageCommandValidator : AbstractValidator<CreateStreetcodeMainPageCommand>
    {
        public CreateStreetcodeMainPageCommandValidator()
        {
            RuleFor(x => x.StreetcodeMainPage)
                .NotNull().WithMessage("StreetcodeMainPage object is required")
                .SetValidator(new StreetcodeMainPageCreateDTOValidator());
        }
    }
}
