using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Timeline.Create;
using Streetcode.BLL.DTO.Timeline.Update;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;

namespace Streetcode.WebApi.Controllers.Timeline
{
    public class TimelineItemController : BaseApiController
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllTimelineItemsQuery()));
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetTimelineItemByIdQuery(id)));
        }

        [Authorize]
        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetTimelineItemsByStreetcodeIdQuery(streetcodeId)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimelineItemCreateDto timeline)
        {
            return HandleResult(await Mediator.Send(new CreateTimelineItemCommand(timeline)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TimelineItemUpdateDto timelineItem)
        {
            return HandleResult(await Mediator.Send(new BLL.MediatR.Timeline.TimelineItem.Update.UpdateTimelineItemCommand(timelineItem)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteTimelineItemCommand(id)));
        }
    }
}