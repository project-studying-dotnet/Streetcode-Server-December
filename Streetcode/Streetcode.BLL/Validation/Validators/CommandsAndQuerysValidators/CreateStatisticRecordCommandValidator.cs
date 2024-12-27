using FluentValidation;
using Streetcode.BLL.MediatR.Analytics;
using Streetcode.BLL.Validation.Validators.DTOValidators.Analytics;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{

    public class CreateStatisticRecordCommandValidator : AbstractValidator<CreateStatisticRecordCommand>
    {
        public CreateStatisticRecordCommandValidator()
        {
            RuleFor(x => x.createStatisticRecord)
                .NotNull().WithMessage("StatisticRecord cannot be null")
                .SetValidator(new CreateStatisticRecordDTOValidator());
        }
    }
}
