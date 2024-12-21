using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete
{
	public class DeleteFactHandler : IRequestHandler<DeleteFactCommand, Result<FactDto>>
	{
		private readonly IRepositoryWrapper _repositoryWrapper;
		private readonly IMapper _mapper;
		private readonly ILoggerService _logger;
		public DeleteFactHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
		{
			_repositoryWrapper = repositoryWrapper;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Result<FactDto>> Handle(DeleteFactCommand request, CancellationToken cancellationToken)
		{
			var fact = await _repositoryWrapper.FactRepository.GetFirstOrDefaultAsync(f => f.Id == request.id);
			if (fact == null)
			{
				var errMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "fact" , request.id);
				_logger.LogError(request, errMsg);
				return Result.Fail(new Error(errMsg));
			}
			else
			{
				_repositoryWrapper.FactRepository.Delete(fact);
				try
				{
					_repositoryWrapper.SaveChanges();
					return Result.Ok(_mapper.Map<FactDto>(fact));
				}
				catch (Exception ex)
				{
					_logger.LogError(request, ex.Message);
					return Result.Fail(ex.Message);
				}
			}
		}
	}
}
