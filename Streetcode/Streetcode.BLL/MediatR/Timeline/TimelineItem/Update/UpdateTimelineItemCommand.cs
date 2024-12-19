using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline.Update;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update
{
    public record UpdateTimelineItemCommand(TimelineItemUpdateDto timelineItemUpdateDto) : IRequest<Result<TimelineItemDTO>>;
}
