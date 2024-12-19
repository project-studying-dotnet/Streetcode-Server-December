using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetAll;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent;


namespace Streetcode.WebApi.Controllers.Source;

public class SourcesController : BaseApiController
{
	[HttpGet]
	public async Task<IActionResult> GetAllNames()
	{
		return HandleResult(await Mediator.Send(new GetAllCategoryNamesQuery()));
	}

	[HttpGet]
	public async Task<IActionResult> GetAllCategories()
	{
		return HandleResult(await Mediator.Send(new GetAllCategoriesQuery()));
	}

	[HttpGet("{id:int}")]
	public async Task<IActionResult> GetCategoryById([FromRoute] int id)
	{
		return HandleResult(await Mediator.Send(new GetCategoryByIdQuery(id)));
	}

	[HttpGet("{categoryId:int}&{streetcodeId:int}")]
	public async Task<IActionResult> GetCategoryContentByStreetcodeId([FromRoute] int streetcodeId, [FromRoute] int categoryId)
	{
		return HandleResult(await Mediator.Send(new GetCategoryContentByStreetcodeIdQuery(streetcodeId, categoryId)));
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteCategoryContent([FromRoute] int id)
	{
		return HandleResult(await Mediator.Send(new DeleteCategoryContentCommand(id)));
  	}

	[HttpGet("{streetcodeId:int}")]
	public async Task<IActionResult> GetCategoriesByStreetcodeId([FromRoute] int streetcodeId)
  	{
  		return HandleResult(await Mediator.Send(new GetCategoriesByStreetcodeIdQuery(streetcodeId)));
  	}
   
	[HttpPut]
	public async Task<IActionResult> UpdateCategories([FromBody] SourceLinkCategoryDTO sourceLinkCategory)
  	{
  		return HandleResult(await Mediator.Send(new UpdateSourceLinkCategoryCommand(sourceLinkCategory)));
  	}

	[HttpPost]
	public async Task<IActionResult> CreateCategoryContent([FromBody] CategoryContentCreateDTO newCategoryContent)
	{
		return HandleResult(await Mediator.Send(new CreateCategoryContentCommand(newCategoryContent)));
	}
}
