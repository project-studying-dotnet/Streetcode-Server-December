using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;

namespace Streetcode.BLL.MediatR.Comment.UserDeleteComment;

public class UserDeleteCommentHandler : IRequestHandler<UserDeleteCommentCommand, Result<Unit>>
{
    private readonly IRepositoryWrapper _repository;
    private readonly ILoggerService _logger;

    public UserDeleteCommentHandler(IRepositoryWrapper repository, ILoggerService logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(UserDeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var existComment = await _repository.CommentRepository.GetFirstOrDefaultBySpecAsync(new CommentWithChildrenSpecification(request.UserDeleteCommentDto.Id));

        if (existComment is null)
        {
            var errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "comment", request.UserDeleteCommentDto.Id);
            _logger.LogError(request, errorMsg);
            throw new Exception(errorMsg);
        }

        if (existComment.UserName != request.UserDeleteCommentDto.UserName)
        {
            var errorMsg = ErrorManager.GetCustomErrorText("DontEqual", "user name", request.UserDeleteCommentDto.UserName);
            _logger.LogError(request, errorMsg);
            throw new Exception(errorMsg);
        }
        
        _repository.CommentRepository
            .Delete(existComment);

        var isDelete = await _repository.SaveChangesAsync() > 0;

        if (isDelete)
        {
            return Result.Ok(Unit.Value);
        }
        else
        {
            var errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "comment", $"comment {request.UserDeleteCommentDto.Id}");
            _logger.LogError(request, errorMsg);
            throw new Exception(errorMsg);
        }
    }
}