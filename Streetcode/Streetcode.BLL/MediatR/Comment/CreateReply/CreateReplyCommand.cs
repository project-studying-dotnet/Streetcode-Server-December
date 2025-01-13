using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comment.CreateReply
{
    public record CreateReplyCommand(CreateReplyDto createReplyDto, string UserName) : IRequest<Result<CreateReplyDto>>
    {
    }
}
