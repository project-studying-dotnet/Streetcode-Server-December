using FluentValidation;
using Streetcode.BLL.MediatR.Comment.UpdateComment;
using Streetcode.BLL.Validation.Validators.DTOValidators.Command;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(x => x.updateCommentDto)
                .NotNull().WithMessage("UpdateCommentDto object is required")
                .SetValidator(new UpdateCommentDtoValidator());
        }
    }
}
