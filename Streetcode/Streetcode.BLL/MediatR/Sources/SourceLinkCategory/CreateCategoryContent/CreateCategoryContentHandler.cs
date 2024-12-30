using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent
{
	public class CreateCategoryContentHandler : IRequestHandler<CreateCategoryContentCommand, Result<CategoryContentCreateDto>>
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

		public async Task<Result<CategoryContentCreateDto>> Handle(CreateCategoryContentCommand request, CancellationToken cancellationToken)
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
				string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "source records");
				_logger.LogError(request, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			var resultDto = _mapper.Map<CategoryContentCreateDto>(newContent);
			return Result.Ok(resultDto);
		}
	}
}
