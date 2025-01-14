using FluentValidation;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Comment.CreateComment;

namespace Streetcode.BLL.Validators.Validators.DTOValidators.Comment;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {        
        RuleFor(c => c.createCommentDto.UserFullName)
            .NotEmpty()
            .WithMessage("User must have a full name");
        
        RuleFor(c => c.createCommentDto.CreatedDate)
            .NotEmpty()
            .WithMessage("Create data must not be empty");
        
        RuleFor(c => c.createCommentDto.Content)
            .NotEmpty()
            .WithMessage("Content is empty");
        
        RuleFor(c => c.createCommentDto.StreetcodeId)
            .NotEmpty()
            .WithMessage("Comment must have streetcode Id")
            .GreaterThan(0)
            .WithMessage("Streetcode cannot be 0");
    }
}