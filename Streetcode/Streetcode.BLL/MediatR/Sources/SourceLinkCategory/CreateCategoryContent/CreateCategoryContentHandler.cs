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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent
{
	public class CreateCategoryContentHandler : IRequestHandler<CreateCategoryContentCommand, Result<CategoryContentCreateDTO>>
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

		public async Task<Result<CategoryContentCreateDTO>> Handle(CreateCategoryContentCommand request, CancellationToken cancellationToken)
		{
			var newContent = _mapper.Map<StreetcodeCategoryContent>(request.newContent);

			newContent = await _repositoryWrapper.StreetcodeCategoryContentRepository.CreateAsync(newContent);

			newContent.SourceLinkCategory = await _repositoryWrapper.SourceCategoryRepository
				.GetSingleOrDefaultAsync(s => s.Id == newContent.SourceLinkCategoryId);
			newContent.Streetcode = await _repositoryWrapper.StreetcodeRepository
				.GetSingleOrDefaultAsync(s => s.Id == newContent.StreetcodeId);

			var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

			if (!resultIsSuccess)
			{
				const string errorMsg = "Failed to create source records";
				_logger.LogError(request, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			var resultDto = _mapper.Map<CategoryContentCreateDTO>(newContent);
			return Result.Ok(resultDto);
		}
	}
}
