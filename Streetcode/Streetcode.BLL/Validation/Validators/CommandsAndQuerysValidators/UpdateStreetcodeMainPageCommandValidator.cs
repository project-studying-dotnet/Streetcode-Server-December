using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.UpdateMainPage;
using Streetcode.BLL.Validation.Validators.DTOValidators.Streetcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class UpdateStreetcodeMainPageCommandValidator : AbstractValidator<UpdateStreetcodeMainPageCommand>
    {
        public UpdateStreetcodeMainPageCommandValidator()
        {
            RuleFor(x => x.StreetcodeMainPage)
                .NotNull().WithMessage("StreetcodeMainPage object is required")
                .SetValidator(new StreetcodeMainPageUpdateDtoValidator());
        }
    }
}
