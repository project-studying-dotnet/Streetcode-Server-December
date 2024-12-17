using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.Interfaces.Logging;
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
            try
            {
                var newStatisticRecord = CreateStatisticRecord(request.createStatisticRecord);

                var entity = _repositoryWrapper.StatisticRecordRepository.Create(
                    newStatisticRecord);

                if (entity is null)
                {
                    return LogAndFail("Cannot convert null to StatisticRecord", request);
                }

                var resultIsSuccess = await _repositoryWrapper.SaveChangesAsync() > 0;

                if (!resultIsSuccess)
                {
                    return LogAndFail("Failed to create a StatisticRecord", request);
                }

                return Result.Ok(_mapper.Map<StatisticRecordDTO>(entity));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating StatisticRecord");
                return Result.Fail(
                    new Error("An unexpected error occurred during record creation"));
            }
        }

        private StatisticRecord CreateStatisticRecord(CreateStatisticRecordDTO createDto)
        {
            var statisticRecord = _mapper.Map<StatisticRecord>(createDto);

            if (statisticRecord is null)
            {
                throw new InvalidOperationException("Mapping resulted in a null StatisticRecord");
            }

            return statisticRecord;
        }

        private Result<StatisticRecordDTO> LogAndFail(string errorMessage, object request = null)
        {
            _logger.LogError(request, errorMessage);
            return Result.Fail(errorMessage);
        }
    }
}