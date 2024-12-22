using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.MediatR.Analytics;
using Streetcode.BLL.MediatR.Analytics.Delete;

namespace Streetcode.WebApi.Controllers.Analytics
{
    public class StatisticRecordController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStatisticRecordDTO record)
        {
            return HandleResult(await Mediator.Send(new CreateStatisticRecordCommand(record)));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteStatisticRecordCommand(id)));
        }
    }
}