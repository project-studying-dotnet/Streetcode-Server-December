using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.Fact.Update;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent
{
    public class FactController : BaseApiController
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllFactsQuery()));
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetFactByIdQuery(id)));
        }

        [Authorize]
        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetFactByStreetcodeIdQuery(streetcodeId)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFactDTO fact)
        {
            return HandleResult(await Mediator.Send(new CreateFactCommand(fact)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FactDto fact)
        {
            return HandleResult(await Mediator.Send(new UpdateFactCommand(fact)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> ReorderFacts([FromBody] FactReorderDto factReorderDto)
        {
            return HandleResult(await Mediator.Send(new FactReorderCommand(factReorderDto)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFact([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteFactCommand(id)));
        }
    }
}