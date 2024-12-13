using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.DTO.Timeline.Create;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create
{
    public record CreateTimelineItemCommand(TImelineItemCreateDto timelineItemCreateDto) : IRequest<Result<TimelineItemDTO>>
    {
    }
}
