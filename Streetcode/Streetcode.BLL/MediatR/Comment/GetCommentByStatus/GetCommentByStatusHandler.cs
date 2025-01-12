using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Comment.GetCommentByStatus
{
    public class GetCommentByStatusHandler : IRequestHandler<GetCommentByStatusCommand, Result<IEnumerable<GetCommentDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;

        public GetCommentByStatusHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<GetCommentDto>>> Handle(GetCommentByStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {      
                var comments = await _repository.CommentRepository
                    .GetAllAsync(c => c.Status == request.Status);
                
                var commentDtos = _mapper.Map<IEnumerable<GetCommentDto>>(comments);

                return Result.Ok(commentDtos);
            }
            catch
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", $"comments with {request.Status} status");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
