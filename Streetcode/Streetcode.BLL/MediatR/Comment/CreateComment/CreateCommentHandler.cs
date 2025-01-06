using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Comment;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Comment.CreateComment;

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<GetCommentDto>>
{
    private IEnumerable<string> prohibitedContent = new List<string>()
    {
        "abc", "123", "456" , "01"
    };

    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly ILoggerService _logger;

    public CreateCommentHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
    {
        _mapper = mapper;
        _repositoryWrapper = repositoryWrapper;
        _logger = logger;
    }
    
    public async Task<Result<GetCommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        CommentStatus status = CommentStatus.Send;

        try
        {
            await _repositoryWrapper.StreetcodeRepository
                .GetFirstOrDefaultAsync(s => s.Id == request.createCommentDto.StreetcodeId);
        }
        catch (Exception e)
        {
            var errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "streetcode",
                request.createCommentDto.StreetcodeId);
            _logger.LogError(request, errMsg);
            return Result.Fail(errMsg);
        }

        var newComment = _mapper.Map<DAL.Entities.Comment.Comment>(request.createCommentDto);

        var words = newComment.Content.Split(new[] { ' ', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        var checkProhibitedContent = words.Any(word => prohibitedContent.Any(prohibited => word.Contains(prohibited)));

        if (newComment is null)
        {
            var errMsg = ErrorManager.GetCustomErrorText("ConvertationError", "create comment dto", "CommentEntity");
            _logger.LogError(request, errMsg);
            return Result.Fail(errMsg);
        }

        if (checkProhibitedContent)
        {
            var prohibitedWords = words.Where(word => prohibitedContent.Contains(word));

            var errMsg = $"You have sensitive words in your message! [{string.Join(", ", prohibitedWords)}]. Your message will be checked by admin.";
            _logger.LogError(request, errMsg);
            newComment.Status = CommentStatus.InReview;
        }

        var result = await _repositoryWrapper.CommentRepository.CreateAsync(newComment);
        
        var resultIsSucceed = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSucceed)
        {
            return Result.Ok(_mapper.Map<GetCommentDto>(result));
        }
        else
        {
            var errMsg = ErrorManager.GetCustomErrorText("FailCreateError", "comment", "");
            _logger.LogError(request, errMsg);
            return Result.Fail(errMsg);
        }
    }
}