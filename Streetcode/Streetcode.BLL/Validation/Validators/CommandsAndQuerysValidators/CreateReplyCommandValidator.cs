using FluentValidation;
using Streetcode.BLL.MediatR.Comment.CreateReply;
using Streetcode.BLL.Validation.Validators.DTOValidators.Comment;


namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators
{
    public class CreateReplyCommandValidator : AbstractValidator<CreateReplyCommand>
    {
        public CreateReplyCommandValidator()
        {
            RuleFor(x => x.createReplyDto)
                .NotNull().WithMessage("CreateReplyDto object is required")
                .SetValidator(new CreateReplyDtoValidator());
        }
    }
}
