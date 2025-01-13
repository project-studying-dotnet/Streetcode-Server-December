﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.RelatedFigure;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog
{
    public class GetAllStreetcodesCatalogHandler : IRequestHandler<GetAllStreetcodesCatalogQuery,
        Result<IEnumerable<RelatedFigureDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllStreetcodesCatalogHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<RelatedFigureDto>>> Handle(GetAllStreetcodesCatalogQuery request, CancellationToken cancellationToken)
        {
            var streetcodes = await _repositoryWrapper.StreetcodeRepository.GetAllAsync(
                predicate: sc => sc.Status == Domain.Enums.StreetcodeStatus.Published,
                include: new List<string> { "Tags", "Images" });

            if (streetcodes != null)
            {
                var skipped = streetcodes.Skip((request.page - 1) * request.count).Take(request.count);
                return Result.Ok(_mapper.Map<IEnumerable<RelatedFigureDto>>(skipped));
            }

            string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "subtitles");
            _logger.LogError(request, errorMsg);
            return Result.Fail(errorMsg);
        }
    }
}
