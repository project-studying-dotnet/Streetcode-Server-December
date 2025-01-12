using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete
{
    public class DeleteSourceLinkCategoryHandler : IRequestHandler<DeleteSourceLinkCategoryCommand, Result<Unit>>
	{
		private readonly IMapper _mapper;
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly ILoggerService _logger;

		public DeleteSourceLinkCategoryHandler(IMapper mapper, IRepositoryWrapper repositoryWrapper, ILoggerService loggerService)
		{
			_mapper = mapper;
			_repositoryWrapper = repositoryWrapper;
			_logger = loggerService;
		}

		public async Task<Result<Unit>> Handle(DeleteSourceLinkCategoryCommand command, CancellationToken cancellationToken)
		{
			var sourceCategory = await _repositoryWrapper.SourceCategoryRepository
				.GetFirstOrDefaultAsync(s => s.Id == command.id);
			if (sourceCategory == null)
			{
				string errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "source", command.id);
				_logger.LogError(command, errMsg);
				return Result.Fail(errMsg);
			}

			_repositoryWrapper.SourceCategoryRepository.Delete(sourceCategory);
			var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

			if (!resultIsSuccess)
			{
				string errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "source");
				_logger.LogError(command, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			_logger.LogInformation("SourceLinkCategory handled successfully");
			return Result.Ok(Unit.Value);
		}
	}
}
