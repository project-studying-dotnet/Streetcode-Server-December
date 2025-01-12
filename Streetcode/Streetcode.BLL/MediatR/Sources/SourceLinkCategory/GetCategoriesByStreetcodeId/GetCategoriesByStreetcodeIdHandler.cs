using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId
{

    public class GetCategoriesByStreetcodeIdHandler : IRequestHandler<GetCategoriesByStreetcodeIdQuery, Result<IEnumerable<SourceLinkCategoryDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;

        public GetCategoriesByStreetcodeIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<SourceLinkCategoryDto>>> Handle(GetCategoriesByStreetcodeIdQuery request, CancellationToken cancellationToken)
        {
            var srcCategories = await _repositoryWrapper
                .SourceCategoryRepository
                .GetAllAsync(
                    predicate: sc => sc.Streetcodes.Any(s => s.Id == request.StreetcodeId),
                    include: scl => scl.Include(sc => sc.Image)!);

            if (srcCategories is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByStreetcodeIdError", "category" , request.StreetcodeId);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var mappedSrcCategories = _mapper.Map<IEnumerable<SourceLinkCategoryDto>>(srcCategories);

            foreach (var srcCategory in mappedSrcCategories)
            {
                srcCategory.Image.Base64 = await _blobService.FindFileInStorageAsBase64(srcCategory.Image.BlobName);
            }

            return Result.Ok(mappedSrcCategories);
        }
    }
}