﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll
{
    public class GetAllCategoryNamesHandler : IRequestHandler<GetAllCategoryNamesQuery, Result<IEnumerable<CategoryWithNameDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllCategoryNamesHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<CategoryWithNameDto>>> Handle(GetAllCategoryNamesQuery request, CancellationToken cancellationToken)
        {
            var allCategories = await _repositoryWrapper.SourceCategoryRepository.GetAllAsync();

            if (allCategories == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "categories");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<CategoryWithNameDto>>(allCategories));
        }
    }
}
