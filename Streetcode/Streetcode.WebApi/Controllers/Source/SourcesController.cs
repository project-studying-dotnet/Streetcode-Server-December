using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent;
using Microsoft.AspNetCore.Authorization;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;

namespace Streetcode.WebApi.Controllers.Source
{
    public class SourcesController : BaseApiController
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllNames()
        {
            return HandleResult(await Mediator.Send(new GetAllCategoryNamesQuery()));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            return HandleResult(await Mediator.Send(new GetAllCategoriesQuery()));
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetCategoryByIdQuery(id)));
        }

        [Authorize]
        [HttpGet("{categoryId:int}&{streetcodeId:int}")]
        public async Task<IActionResult> GetCategoryContentByStreetcodeId([FromRoute] int streetcodeId, [FromRoute] int categoryId)
        {
            return HandleResult(await Mediator.Send(new GetCategoryContentByStreetcodeIdQuery(streetcodeId, categoryId)));
        }

        [Authorize]
        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetCategoriesByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetCategoriesByStreetcodeIdQuery(streetcodeId)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateCategories([FromBody] SourceLinkCategoryDto sourceLinkCategory)
        {
            return HandleResult(await Mediator.Send(new UpdateSourceLinkCategoryCommand(sourceLinkCategory)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteSourceLinkCategoryCommand(id)));
        }

        [AuthorizeRoles(UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] SourceLinkCategoryDto sourceLinkCategory)
        {
            return HandleResult(await Mediator.Send(new CreateSourceLinkCategoryCommand(sourceLinkCategory)));
        }
    }
}