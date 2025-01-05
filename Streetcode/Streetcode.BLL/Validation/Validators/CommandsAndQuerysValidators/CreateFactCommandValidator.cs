using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
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
            RuleFor(command => command.Fact)
                .NotNull()
                .WithMessage("Fact data cannot be null.");

            RuleFor(command => command.Fact)
                .SetValidator(new CreateFactDtoValidator());
        }
    }
}
