using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Repositories.Interfaces.Base;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById
{
    public class GetTimelineItemByIdHandler : IRequestHandler<GetTimelineItemByIdQuery, Result<TimelineItemDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILoggerService _logger;

        public GetTimelineItemByIdHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<TimelineItemDto>> Handle(GetTimelineItemByIdQuery request, CancellationToken cancellationToken)
        {
            var timelineItem = await _repositoryWrapper.TimelineRepository
                .GetFirstOrDefaultAsync(
                    predicate: ti => true,
                    include: new List<string> { "HistoricalContextTimelines.HistoricalContext" });

            if (timelineItem is null)
            {
                string errorMsg = ErrorManager.GetCustomErrorText("CantFindByIdError", "timeline", request.Id);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<TimelineItemDto>(timelineItem));
        }
    }
}
