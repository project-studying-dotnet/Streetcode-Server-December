using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comment.UserDeleteComment;

public record UserDeleteCommentCommand(UserDeleteCommentDto UserDeleteCommentDto) : IRequest<Result<Unit>>;