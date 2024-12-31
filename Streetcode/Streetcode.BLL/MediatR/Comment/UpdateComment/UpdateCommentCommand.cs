using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comment.UpdateComment
{
    public record UpdateCommentCommand(UpdateCommentDto updateCommentDto) : IRequest<Result<GetCommentDto>>;
}
