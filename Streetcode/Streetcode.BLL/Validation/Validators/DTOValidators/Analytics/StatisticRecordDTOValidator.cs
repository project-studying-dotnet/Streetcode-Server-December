using FluentValidation;
using Streetcode.BLL.DTO.Analytics;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Analytics
{
    public class StatisticRecordDTOValidator : AbstractValidator<CreateStatisticRecordDto>
    {
        public StatisticRecordDTOValidator()
        {
            Include(new CreateStatisticRecordDTOValidator());
        }
    }
}
