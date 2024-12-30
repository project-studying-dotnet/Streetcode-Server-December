using FluentValidation;
using Streetcode.BLL.DTO.Analytics;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Analytics
{
    public class CreateStatisticRecordDTOValidator : AbstractValidator<CreateStatisticRecordDto>
    {
        public CreateStatisticRecordDTOValidator()
        {
            RuleFor(x => x.QrId)
           .GreaterThan(0)
           .WithMessage("QrId must be greater than 0");

            RuleFor(x => x.Count)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Count cannot be negative");

            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address cannot be empty")
                .MaximumLength(300)
                .WithMessage("Address cannot exceed 300 characters");

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage("StreetcodeId must be greater than zero");

            RuleFor(x => x.StreetcodeCoordinateId)
                .GreaterThan(0)
                .WithMessage("StreetcodeCoordinateId must be greater than 0");
        }
    }
}
