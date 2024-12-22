using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Microsoft.AspNetCore.Authorization;

namespace Streetcode.WebApi.Controllers.Media.Images
{
    public class ArtController : BaseApiController
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllArtsQuery()));
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetArtByIdQuery(id)));
        }

        [Authorize]
        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetArtsByStreetcodeIdQuery(streetcodeId)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArtCreateDTO art)
        {
            return HandleResult(await Mediator.Send(new CreateArtCommand(art)));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteArtCommand(id)));
        }
    }
}