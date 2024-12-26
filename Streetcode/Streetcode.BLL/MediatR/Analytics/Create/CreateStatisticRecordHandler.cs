using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Analytics;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Analytics
{
    public class CreateStatisticRecordHandler
        : IRequestHandler<CreateStatisticRecordCommand, Result<StatisticRecordDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public CreateStatisticRecordHandler(
            IMapper mapper,
            IRepositoryWrapper repositoryWrapper,
            ILoggerService logger)
        {
            _mapper = mapper;
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        public async Task<Result<StatisticRecordDTO>> Handle(
            CreateStatisticRecordCommand request,
            CancellationToken cancellationToken)
        {
            var newStatisticRecord = _mapper.Map<StatisticRecord>(request.createStatisticRecord);

            var entity = await _repositoryWrapper.StatisticRecordRepository.CreateAsync(
                newStatisticRecord);

            if (entity is null)
            {
                return LogAndFail(ErrorManager.GetCustomErrorText("ConvertationError", "null", "StatisticRecord"), request);
            }

            var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

            if (!resultIsSuccess)
            {
                return LogAndFail(ErrorManager.GetCustomErrorText("FailCreateError", "StatisticRecord"), request);
            }

            return Result.Ok(_mapper.Map<StatisticRecordDTO>(entity));
        }

        private Result<StatisticRecordDTO> LogAndFail(string errorMessage, object request = null)
        {
            _logger.LogError(request, errorMessage);
            return Result.Fail(errorMessage);
        }
    }
}