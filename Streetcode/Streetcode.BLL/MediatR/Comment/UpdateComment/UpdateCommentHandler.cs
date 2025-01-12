using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using CommentEntity = Streetcode.Domain.Entities.Comment.Comment;

namespace Streetcode.BLL.MediatR.Comment.UpdateComment
{
    public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, Result<GetCommentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public UpdateCommentHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<GetCommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _repositoryWrapper.StreetcodeRepository
                    .GetFirstOrDefaultAsync(s => s.Id == request.updateCommentDto.StreetcodeId);
            }
            catch (Exception e)
            {
                var errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "streetcode", request.updateCommentDto.StreetcodeId);
                _logger.LogError(request, errMsg);
                return Result.Fail(errMsg);
            }

            var updatedComment = _mapper.Map<CommentEntity>(request.updateCommentDto); 

            if(updatedComment == null)
            {
                var errMsg = ErrorManager.GetCustomErrorText("ConvertationError", "UpdateCommandDto", "Comment");
                _logger.LogError(request, errMsg);
                return Result.Fail(errMsg);
            }
            else
            {
                var currComment = await _repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
                    predicate: comment => comment.CreatedDate == updatedComment.CreatedDate && comment.UserFullName == updatedComment.UserFullName
                );

                if(currComment == null)
                {
                    var errMsg = $"Cannot find any comments with corresponding values [Create:{updatedComment.CreatedDate} - by {updatedComment.UserFullName}]";
                    _logger.LogError(request, errMsg);
                    return Result.Fail(errMsg);
                }
                else
                {
                    currComment.DateModified = request.updateCommentDto.DateModified;
                    currComment.Content = request.updateCommentDto.Content;

                    var result1 = _repositoryWrapper.CommentRepository.Update(currComment);
                    await _repositoryWrapper.SaveChangesAsync();
                }
            }

            var result = await _repositoryWrapper.CommentRepository.GetFirstOrDefaultAsync(
                   predicate: comment => comment.CreatedDate == updatedComment.CreatedDate && comment.UserFullName == updatedComment.UserFullName && comment.Content == updatedComment.Content
               );

            if (result != null)
            {
                return Result.Ok(_mapper.Map<GetCommentDto>(result));
            }
            else
            {
                var errMsg = ErrorManager.GetCustomErrorText("FailUpdateError", "comment", "");
                _logger.LogError(request, errMsg);
                return Result.Fail(errMsg);
            }
        }
    }
}
