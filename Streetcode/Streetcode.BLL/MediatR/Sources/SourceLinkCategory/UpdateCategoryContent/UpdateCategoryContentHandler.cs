using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.Domain.Entities.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateCategoryContent
{
    public class UpdateCategoryContentHandler : IRequestHandler<UpdateCategoryContentCommand, Result<CategoryContentCreateDto>>
	{
		private readonly IMapper _mapper;
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly ILoggerService _logger;

		public UpdateCategoryContentHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService loggerService)
		{
			_mapper = mapper;
			_repositoryWrapper = repositoryWrapper;
			_logger = loggerService;
		}

		public async Task<Result<CategoryContentCreateDto>> Handle(UpdateCategoryContentCommand request, CancellationToken cancellationToken)
		{
			var source = _mapper.Map<StreetcodeCategoryContent>(request.CategoryContent);
			if (source == null)
			{
				const string errMsg = "Source category content no found!";
				_logger.LogError(request, errMsg);
				return Result.Fail(new Error(errMsg));
			}

			try
			{
				var existCategoryContent = await _repositoryWrapper.StreetcodeCategoryContentRepository
					.GetFirstOrDefaultAsync(c => c.Id == source.Id);
			}
			catch (Exception ex)
			{
				const string errorMsg = $"Cannot find source category content in Db";
				_logger.LogError(request, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			_repositoryWrapper.StreetcodeCategoryContentRepository.Update(source);

			var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

			if (resultIsSuccess)
			{
				return Result.Ok(_mapper.Map<CategoryContentCreateDto>(source));
			}
			else
			{
				const string errorMsg = $"Failed to update category content";
				_logger.LogError(request, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}
		}
	}
}
