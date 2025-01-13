using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Specifications.Comment;
using Streetcode.DAL.Repositories.Interfaces.Base;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.BLL.MediatR.Comment.CreateReply
{
    public class CreateReplyHandler : IRequestHandler<CreateReplyCommand, Result<CreateReplyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateReplyHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<CreateReplyDto>> Handle(CreateReplyCommand request, CancellationToken cancellationToken)
        {
            var existingParent = await _repositoryWrapper.CommentRepository
                    .GetFirstOrDefaultBySpecAsync(new CommentByParentIdSpecification(request.createReplyDto.ParentId));

            if (existingParent is null)
            {
                var errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "comment", request.createReplyDto.ParentId);
                _logger.LogError(request, errorMsg);
                return Result.Fail(errorMsg);
            }

            var newReply = _mapper.Map<CommentEntity>(request.createReplyDto);

            if (newReply is null)
            {
                var errMsg = ErrorManager.GetCustomErrorText("ConvertationError", "create reply dto", "CommentEntity");
                _logger.LogError(request, errMsg);
                return Result.Fail(errMsg);
            }

            newReply.UserName = request.UserName;

            var result = await _repositoryWrapper.CommentRepository.CreateAsync(newReply);

            var resultIsSucceed = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSucceed)
            {
                var errMsg = ErrorManager.GetCustomErrorText("FailCreateError", "reply", "");
                _logger.LogError(request, errMsg);
                return Result.Fail(errMsg);
            }

            return Result.Ok(_mapper.Map<CreateReplyDto>(result));
        }
    }
}
