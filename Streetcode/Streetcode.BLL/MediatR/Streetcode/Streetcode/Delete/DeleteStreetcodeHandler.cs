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

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Delete
{
	public class DeleteStreetcodeHandler : IRequestHandler<DeleteStreetcodeCommand, Result<Unit>>
	{
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly ILoggerService _logger;

		public DeleteStreetcodeHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
		{
			_repositoryWrapper = repositoryWrapper;
			_logger = logger;
		}

		public async Task<Result<Unit>> Handle(DeleteStreetcodeCommand command, CancellationToken cancellationToken)
		{
			var streetcode = await _repositoryWrapper.StreetcodeRepository
				.GetFirstOrDefaultAsync(s => s.Id == command.id);

			if (streetcode == null)
			{
				string errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "streetcode", command.id);
				_logger.LogError(command, errMsg);
				return Result.Fail(errMsg);
			}

			_repositoryWrapper.StreetcodeRepository.Delete(streetcode);

			var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

			if (!resultIsSuccess)
			{
				string errorMsg = ErrorManager.GetCustomErrorText("FailDeleteError", "streetcode");
				_logger.LogError(command, errorMsg);
				return Result.Fail(new Error(errorMsg));
			}

			_logger.LogInformation("Streetcode handled successfully");
			return Result.Ok(Unit.Value);
		}
	}
}
