using FluentValidation;
using Streetcode.BLL.MediatR.Comment.AdminDeleteComment;
using Streetcode.BLL.MediatR.Comment.UserDeleteComment;

namespace Streetcode.BLL.Validation.Validators.CommandsAndQuerysValidators;

public class DeleteCommentCommandValidator : AbstractValidator<UserDeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(c => c.UserDeleteCommentDto.Id)
            .NotNull()
            .WithMessage("Id cannot be null")
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");

        RuleFor(x => x.UserDeleteCommentDto.UserName)
            .NotNull()
            .WithMessage("User name cannot be null");
    }
}