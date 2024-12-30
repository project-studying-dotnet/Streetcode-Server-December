using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comment.CreateComment;
public record CreateCommentCommand(CreateCommentDto createCommentDto) : IRequest<Result<GetCommentDto>>;