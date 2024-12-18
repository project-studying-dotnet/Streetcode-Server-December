using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.MediatR.Analytics;

namespace Streetcode.WebApi.Controllers.Analytics
{
    public class StatisticRecordController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStatisticRecordDTO record)
        {
            return HandleResult(await Mediator.Send(new CreateStatisticRecordCommand(record)));
        }
    }
}
