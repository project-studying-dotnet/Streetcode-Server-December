using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.Validation.Validators.DTOValidators.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateFactCommandValidator : AbstractValidator<CreateFactCommand>
    {
        public CreateFactCommandValidator()
        {
            RuleFor(x => x.Fact)
                .NotNull().WithMessage("Fact cannot be null.")
                .SetValidator(new CreateFactDTOValidator());
        }
    }
}
