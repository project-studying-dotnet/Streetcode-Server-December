using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll
{

    public class GetAllTimelineItemsHandler : IRequestHandler<GetAllTimelineItemsQuery, Result<IEnumerable<TimelineItemDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetAllTimelineItemsHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<TimelineItemDto>>> Handle(GetAllTimelineItemsQuery request, CancellationToken cancellationToken)
        {
            var timelineItems = await _repositoryWrapper
                .TimelineRepository.GetAllAsync(
                    include: new List<string> { "HistoricalContextTimelines.HistoricalContext" });

            if (timelineItems is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindError", "timeline");
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<IEnumerable<TimelineItemDto>>(timelineItems));
        }
    }
}
