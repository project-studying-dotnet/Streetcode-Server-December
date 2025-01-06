using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.MediatR.Comment.GetCommentByStatus
{
    public record GetCommentByStatusCommand(CommentStatus Status) : IRequest<Result<IEnumerable<GetCommentDto>>>;
}
