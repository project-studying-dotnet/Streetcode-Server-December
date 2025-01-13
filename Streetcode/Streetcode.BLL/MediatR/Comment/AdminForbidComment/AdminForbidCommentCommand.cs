using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Comment.AdminForbidComment
{
    public record AdminForbidCommentCommand(int Id) : IRequest<Result<string>>;
}
