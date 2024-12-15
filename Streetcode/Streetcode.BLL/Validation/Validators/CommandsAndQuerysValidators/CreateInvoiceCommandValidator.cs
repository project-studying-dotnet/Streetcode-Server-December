using FluentValidation;
using Streetcode.BLL.MediatR.Payment;
using Streetcode.BLL.Validation.Validators.DTOValidators.Payment;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceCommandValidator()
        {
            RuleFor(x => x.Payment)
                .NotNull().WithMessage("Payment cannot be null")
                .SetValidator(new PaymentDTOValidator());
        }
    }
}
