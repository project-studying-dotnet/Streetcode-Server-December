using FluentResults;
using MediatR;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Analytics.Delete
{
    public class DeleteStatisticRecordHandler : IRequestHandler<DeleteStatisticRecordCommand, Result<bool>>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public DeleteStatisticRecordHandler(IRepositoryWrapper repositoryWrapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteStatisticRecordCommand request, CancellationToken cancellationToken)
        {
            var statisticRecordItem = await _repositoryWrapper.StatisticRecordRepository.GetFirstOrDefaultAsync(f => f.Id == request.id);

            if (statisticRecordItem == null)
            {
                string errorMsg = $"Cannot find statisticRecord item with Id: {request.id}";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            _repositoryWrapper.StatisticRecordRepository.Delete(statisticRecordItem);

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                const string errorMsg = $"Failed to delete statisticRecord item";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(true);
        }
    }
}