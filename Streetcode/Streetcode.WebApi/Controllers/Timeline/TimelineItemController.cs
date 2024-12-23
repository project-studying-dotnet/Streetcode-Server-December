using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.DTO.Timeline.Update;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Timeline
{
    public class TimelineItemController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllTimelineItemsQuery()));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetTimelineItemByIdQuery(id)));
        }

        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimelineItemCreateDto timeline)
        {
            return HandleResult(await Mediator.Send(new CreateTimelineItemCommand(timeline)));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TimelineItemUpdateDto timelineItem)
        {
            return HandleResult(await Mediator.Send(new BLL.MediatR.Timeline.TimelineItem.Update.UpdateTimelineItemCommand(timelineItem)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteTimelineItemCommand(id)));
        }
    }
}