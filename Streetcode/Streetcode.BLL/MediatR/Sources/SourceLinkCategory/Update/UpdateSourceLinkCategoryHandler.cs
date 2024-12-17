using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;

public class UpdateSourceLinkCategoryHandler : IRequestHandler<UpdateSourceLinkCategoryCommand, Result<SourceLinkCategoryDTO>>
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

    public async Task<Result<SourceLinkCategoryDTO>> Handle(UpdateSourceLinkCategoryCommand request, CancellationToken cancellationToken)
    {
        var sourceLinkCategory = _mapper.Map<DAL.Entities.Sources.SourceLinkCategory>(request.SourceLinkCategory);
        if (sourceLinkCategory is null)
        {
            const string errorMsg = $"Cannot convert null to source link category";
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
            const string errorMsg = $"Cannot find source link category in db";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        // if (existSourceLinkCategory is null)
        // {
        //     
        // }

        _repositoryWrapper.SourceCategoryRepository.Update(sourceLinkCategory);
        var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

        if (resultIsSuccess)
        {
            return Result.Ok(_mapper.Map<SourceLinkCategoryDTO>(sourceLinkCategory));
        }
        else
        {
            const string errorMsg = $"Failed to update news";
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }
    }
}