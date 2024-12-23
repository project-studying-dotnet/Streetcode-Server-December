using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.Interfaces.Logging;
using TimelineEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create
{
    public class CreateTimelineItemHandler : IRequestHandler<CreateTimelineItemCommand, Result<TimelineItemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapper _repository;
        private readonly ILoggerService _logger;
        public CreateTimelineItemHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<TimelineItemDTO>> Handle(CreateTimelineItemCommand request, CancellationToken cancellationToken)
        {
            var streetcodeExists = await _repository.StreetcodeRepository.GetFirstOrDefaultAsync(s => s.Id == request.timelineItemCreateDto.StreetcodeId);
            if (streetcodeExists == null)
            {
                string errorMessage = ErrorManager.GetCustomErrorText("CantFindError", "streetcode");
                _logger.LogError(request, errorMessage);
                return Result.Fail(errorMessage);
            }

            var newTimelineItem = _mapper.Map<TimelineEntity>(request.timelineItemCreateDto);

            if (newTimelineItem == null)
            {
                string errorMessage = ErrorManager.GetCustomErrorText("FailCreateError", "timeline");
                _logger.LogError(request, errorMessage);
                return Result.Fail(errorMessage);
            }

            // if historical context doesnt exist create timeline without it
            if (!request.timelineItemCreateDto.HistoricalContexts!.Any())
            {
                var createResult = await _repository.TimelineRepository.CreateAsync(newTimelineItem);
                var saveResult = await _repository.SaveChangesAsync();
                return Result.Ok(_mapper.Map<TimelineItemDTO>(newTimelineItem));
            }

            newTimelineItem.HistoricalContextTimelines.Clear();
            await _repository.TimelineRepository.CreateAsync(newTimelineItem);
            await _repository.SaveChangesAsync();

            var historicalContextIds = request.timelineItemCreateDto.HistoricalContexts.Select(hc => hc.Id).ToList();

            var historicalContexts = await _repository.HistoricalContextRepository
                .GetAllAsync(hc => historicalContextIds.Contains(hc.Id));

            var historicalContextTimelines = historicalContexts.
                Select(hct => new HistoricalContextTimeline
                {
                    HistoricalContextId = hct.Id,
                    HistoricalContext = hct,
                    TimelineId = newTimelineItem.Id,
                    Timeline = newTimelineItem
                }).ToList();

            newTimelineItem.HistoricalContextTimelines.AddRange(historicalContextTimelines);
            await _repository.SaveChangesAsync();

            return Result.Ok(_mapper.Map<TimelineItemDTO>(newTimelineItem));
        }
        }
    }
