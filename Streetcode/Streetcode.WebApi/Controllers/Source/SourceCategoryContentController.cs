using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent;
using Microsoft.AspNetCore.Authorization;
using UserService.BLL.Attributes;
using UserService.DAL.Enums;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.UpdateCategoryContent;

namespace Streetcode.WebApi.Controllers.Source
{
	public class SourceCategoryContentController : BaseApiController
	{
		[AuthorizeRoles(UserRole.Admin)]
		[HttpPost]
		public async Task<IActionResult> CreateCategoryContent([FromBody] CategoryContentCreateDto newCategoryContent)
		{
			return HandleResult(await Mediator.Send(new CreateCategoryContentCommand(newCategoryContent)));
		}

		[AuthorizeRoles(UserRole.Admin)]
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteCategoryContent([FromRoute] int id)
		{
			return HandleResult(await Mediator.Send(new DeleteCategoryContentCommand(id)));
		}

		[AuthorizeRoles(UserRole.Admin)]
		[HttpPut]
		public async Task<IActionResult> UpdateCategoryContent([FromBody] CategoryContentCreateDTO categoryContent)
		{
			return HandleResult(await Mediator.Send(new UpdateCategoryContentCommand(categoryContent)));
		}
	}
}
