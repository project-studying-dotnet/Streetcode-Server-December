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
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Media.Images;
using SourceCategory = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create
{
	public class CreateSourceLinkCategoryHandler : IRequestHandler<CreateSourceLinkCategoryCommand, Result<SourceLinkCategoryDTO>>
	{
		private readonly IMapper _mapper;
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly ILoggerService _logger;

		public CreateSourceLinkCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService loggerService)
		{
			_mapper = mapper;
			_repositoryWrapper = repositoryWrapper;
			_logger = loggerService;
		}

		public async Task<Result<SourceLinkCategoryDTO>> Handle(CreateSourceLinkCategoryCommand command, CancellationToken cancellationToken)
		{
			var image = await _repositoryWrapper.ImageRepository
				.GetFirstOrDefaultAsync(i => i.Id == command.SourceLinkCategory.ImageId);

			if (image is null)
			{
				string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "Image", "source link category");
				_logger.LogError(command, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			try
			{
				var newCategory = _mapper.Map<SourceCategory>(command.SourceLinkCategory);

				newCategory = await _repositoryWrapper.SourceCategoryRepository.CreateAsync(newCategory);

				var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;
				if (!resultIsSuccess)
				{
					string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "source records");
					_logger.LogError(command, errorMsg);
					return Result.Fail(new Error(errorMsg));
				}

				var resultDto = _mapper.Map<SourceLinkCategoryDTO>(newCategory);
				if (image != null)
				{
					resultDto.Image = _mapper.Map<ImageDTO>(image);
				}

				return Result.Ok(resultDto);
			}
			catch (Exception ex)
			{
				string errorMsg = ErrorManager.GetCustomErrorText("FailCreateError", "source link");
				_logger.LogError(command, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}
		}
	}
}
