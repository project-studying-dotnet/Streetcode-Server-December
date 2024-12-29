using FluentValidation;
using Streetcode.BLL.DTO.Payment;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Payment
{
    public class PaymentDTOValidator : AbstractValidator<PaymentDto>
    {
        public PaymentDTOValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.RedirectUrl)
                .Matches(@"^(http|https)://").When(x => !string.IsNullOrEmpty(x.RedirectUrl))
                .WithMessage("RedirectUrl must be a valid URL starting with http:// or https://");
        }
    }
}
