using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.Fact
{
    public class FactController : BaseApiController
    {
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FactDto fact)
        {
            return HandleResult(await Mediator.Send(new UpdateFactCommand(fact)));
        }
    }
}
