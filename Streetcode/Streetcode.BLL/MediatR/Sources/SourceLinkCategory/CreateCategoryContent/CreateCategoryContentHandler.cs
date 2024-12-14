using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent
{
	public class CreateCategoryContentHandler : IRequestHandler<CreateCategoryContentCommand, Result<StreetcodeCategoryContentDTO>>
	{
		private readonly IMapper _mapper;
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly ILoggerService _logger;

		public CreateCategoryContentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
		{
			_repositoryWrapper = repositoryWrapper;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Result<StreetcodeCategoryContentDTO>> Handle(CreateCategoryContentCommand request, CancellationToken cancellationToken)
		{
			var newContent = _mapper.Map<StreetcodeCategoryContent>(request.newContent);

			try
			{
				newContent = await _repositoryWrapper.StreetcodeCategoryContentRepository.CreateAsync(newContent);

				newContent.SourceLinkCategory = await _repositoryWrapper.SourceCategoryRepository.GetSingleOrDefaultAsync(s => s.Id == newContent.SourceLinkCategoryId);
				newContent.Streetcode = await _repositoryWrapper.StreetcodeRepository.GetSingleOrDefaultAsync(s => s.Id == newContent.StreetcodeId);

				_repositoryWrapper.SaveChanges();

				var resultDto = _mapper.Map<StreetcodeCategoryContentDTO>(newContent);
				return Result.Ok(resultDto);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error occurred while creating StreetcodeCategoryContent: {ex.Message}", ex.Message);
				return Result.Fail("An error occurred while creating the content.");
			}
		}
	}
}
