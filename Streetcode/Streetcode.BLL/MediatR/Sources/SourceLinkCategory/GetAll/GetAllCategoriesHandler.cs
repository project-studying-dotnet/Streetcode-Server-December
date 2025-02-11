﻿using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, Result<IEnumerable<SourceLinkCategoryDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IBlobService _blobService;
        private readonly ILoggerService _logger;
        public GetAllCategoriesHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, IBlobService blobService, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<SourceLinkCategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var allCategories = await _repositoryWrapper.SourceCategoryRepository.GetAllAsync(
                include: cat => cat.Include(img => img.Image) !);
            if (allCategories == null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "categories");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var dtos = _mapper.Map<IEnumerable<SourceLinkCategoryDto>>(allCategories);

            foreach (var dto in dtos)
            {
                dto.Image.Base64 = await _blobService.FindFileInStorageAsBase64(dto.Image.BlobName);
            }

            return Result.Ok(dtos);
        }
    }
}
