using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.MediatR.Media.Image.GetAll;
using Streetcode.BLL.MediatR.Media.Image.GetBaseImage;
using Streetcode.BLL.MediatR.Media.Image.GetById;
using Streetcode.BLL.MediatR.Media.Image.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.BLL.MediatR.Media.Image.Delete;

namespace Streetcode.WebApi.Controllers.Media.Images
{
    public class ImageController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllImagesQuery()));
        }

        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetImageByStreetcodeIdQuery(streetcodeId)));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetImageByIdQuery(id)));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImageFileBaseCreateDto image)
        {
            return HandleResult(await Mediator.Send(new CreateImageCommand(image)));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteImageCommand(id)));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBaseImage([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetBaseImageQuery(id)));
        }
    }
}