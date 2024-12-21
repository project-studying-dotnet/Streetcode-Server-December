using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent
{
	public class DeleteCategoryContentHandler : IRequestHandler<DeleteCategoryContentCommand, Result<Unit>>
	{
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly IMapper _mapper;
		private readonly ILoggerService _logger;

		public DeleteCategoryContentHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
		{
			_repositoryWrapper = repositoryWrapper;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Result<Unit>> Handle(DeleteCategoryContentCommand command, CancellationToken cancellationToken)
		{
			var source = await _repositoryWrapper.StreetcodeCategoryContentRepository
				.GetFirstOrDefaultAsync(s => s.Id == command.id);
			if (source == null)
			{
				string errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "source", command.id);
				_logger.LogError(command, errMsg);
				return Result.Fail(errMsg);
			}

			_repositoryWrapper.StreetcodeCategoryContentRepository.Delete(source);
			var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

			if (!resultIsSuccess)
			{
				string errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "source");
				_logger.LogError(command, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			_logger.LogInformation("SourceCategoryContent handled successfully");
			return Result.Ok(Unit.Value);
		}
	}
}
