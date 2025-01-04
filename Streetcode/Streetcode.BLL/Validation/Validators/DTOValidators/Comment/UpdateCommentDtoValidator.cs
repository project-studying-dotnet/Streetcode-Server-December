using FluentValidation;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Command
{
    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.UserFullName)
             .NotEmpty().WithMessage("UserFullName is required.")
             .MaximumLength(100).WithMessage("UserFullName cannot be longer than 100 symbols");

            RuleFor(x => x.CreatedDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("CreatedDate cannot be in the future");

            RuleFor(x => x.DateModified)
                .GreaterThanOrEqualTo(x => x.CreatedDate).WithMessage("DateModified cannot be earlier than CreatedDate");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty")
                .MaximumLength(100).WithMessage("Content cannot be longer than 100 symbols");

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0).WithMessage("StreetcodeId must be a positive number.");
        }
    }
}
