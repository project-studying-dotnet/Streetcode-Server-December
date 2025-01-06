using FluentValidation;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.Validation.Validators.DTOValidators.Comment
{
    public class CreateReplyDtoValidator : AbstractValidator<CreateReplyDto>
    {
        public CreateReplyDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");

            RuleFor(x => x.UserFullName)
                .NotEmpty().WithMessage("UserFullName is required.")
                .MaximumLength(100).WithMessage("UserFullName must not exceed 100 characters.");

            RuleFor(x => x.CreatedDate)
                .NotEmpty().WithMessage("CreatedDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("CreatedDate cannot be in the future.");

            RuleFor(x => x.DateModified)
                .GreaterThanOrEqualTo(x => x.CreatedDate).WithMessage("DateModified cannot be earlier than CreatedDate.")
                .When(x => x.DateModified.HasValue); // Only validate if DateModified is not null

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(500).WithMessage("Content must not exceed 500 characters.");

            RuleFor(x => x.StreetcodeId)
                .NotEmpty().WithMessage("Reply must have streetcode Id")
                .GreaterThan(0).WithMessage("StreetcodeId must be greater than 0.");

            RuleFor(x => x.ParentId)
                .NotEmpty().WithMessage("Reply must have ParentId")
                .GreaterThan(0).WithMessage("ParentId must be greater than 0.");
        }
    }
}
