using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Comment.GetCommentsToReview
{
    public class GetCommentsToReviewHandler : IRequestHandler<GetCommentsToReviewQuery, Result<IEnumerable<GetCommentsToReviewDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public GetCommentsToReviewHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<GetCommentsToReviewDto>>> Handle(GetCommentsToReviewQuery request, CancellationToken cancellationToken)
        {
            var comments = await _repository.CommentRepository
                .GetAllAsync(g => request.restrictedWords.Any(a => g.Content.Contains(a)));

            if (comments is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "comments");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var commentsToReviewDto = _mapper.Map<IEnumerable<GetCommentsToReviewDto>>(comments);

            return Result.Ok(commentsToReviewDto);
        }
    }
}
