using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update
{
    public class UpdateTimelineItemHandler : IRequestHandler<UpdateTimelineItemCommand, Result<TimelineItemDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;
        public UpdateTimelineItemHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _repository = repositoryWrapper;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<Result<TimelineItemDto>> Handle(UpdateTimelineItemCommand request, CancellationToken cancellationToken)
        {
            var timelineItemDto = request.timelineItemUpdateDto;

            // Проверка существования элемента
            var updatingTimelineItem = await _repository.TimelineRepository.GetFirstOrDefaultAsync(
                predicate: t => t.Id == timelineItemDto.Id,
                include: i => i.Include(t => t.HistoricalContextTimelines));

            if (updatingTimelineItem == null)
            {
                string errorMessage = ErrorManager.GetCustomErrorText("CantFindError", "timeline");
                _logger.LogError(request, errorMessage);
                return Result.Fail(errorMessage);
            }
            
            _mapper.Map(timelineItemDto, updatingTimelineItem);

            // Clear historical context timelines
            updatingTimelineItem.HistoricalContextTimelines.Clear();

            // Update historical context timelines
            if (timelineItemDto.HistoricalContexts != null && timelineItemDto.HistoricalContexts.Any())
            {
                var historicalContextIds = timelineItemDto.HistoricalContexts.Select(hc => hc.Id).ToList();

                var historicalContexts = await _repository.HistoricalContextRepository
                    .GetAllAsync(hc => historicalContextIds.Contains(hc.Id));

                var historicalContextTimelines = historicalContexts
                    .Select(hc => new HistoricalContextTimeline
                    {
                        HistoricalContextId = hc.Id,
                        HistoricalContext = hc,
                        TimelineId = updatingTimelineItem.Id,
                        Timeline = updatingTimelineItem
                    }).ToList();

                updatingTimelineItem.HistoricalContextTimelines.AddRange(historicalContextTimelines);
            }

            _repository.TimelineRepository.Update(updatingTimelineItem);
            var saveResult = await _repository.SaveChangesAsync();

            if (saveResult == 0)
            {
                string errorMessage = ErrorManager.GetCustomErrorText("FailUpdateError", "timeline");
                _logger.LogError(request, errorMessage);
                return Result.Fail(errorMessage);
            }

            return Result.Ok(_mapper.Map<TimelineItemDto>(updatingTimelineItem));
        }
    }
}