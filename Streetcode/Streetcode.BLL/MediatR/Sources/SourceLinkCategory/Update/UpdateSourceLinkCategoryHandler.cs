using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update
{
    public class UpdateSourceLinkCategoryHandler : IRequestHandler<UpdateSourceLinkCategoryCommand, Result<SourceLinkCategoryDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public UpdateSourceLinkCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService loggerService)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = loggerService;
        }

        public async Task<Result<SourceLinkCategoryDto>> Handle(UpdateSourceLinkCategoryCommand request, CancellationToken cancellationToken)
        {
            var sourceLinkCategory = _mapper.Map<Streetcode.Domain.Entities.Sources.SourceLinkCategory>(request.SourceLinkCategory);
            if (sourceLinkCategory is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("ConvertationError", "null", "source link category");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            try
            {
                var existSourceLinkCategory =
                    await _repositoryWrapper.SourceCategoryRepository.GetFirstOrDefaultAsync(s =>
                        s.Id == sourceLinkCategory.Id);
            }
            catch (Exception e)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "source link category");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.SourceCategoryRepository.Update(sourceLinkCategory);
            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (resultIsSuccess)
            {
                return Result.Ok(_mapper.Map<SourceLinkCategoryDto>(sourceLinkCategory));
            }
            else
            {
                string errorMsg = ErrorManager.GetCustomErrorText("FailUpdateError", "source link");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
