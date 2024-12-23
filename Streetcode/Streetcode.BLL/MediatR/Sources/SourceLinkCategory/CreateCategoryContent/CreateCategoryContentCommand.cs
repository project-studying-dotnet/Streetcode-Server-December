using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;


namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.CreateCategoryContent
{
	public record CreateCategoryContentCommand(CategoryContentCreateDTO newContent) : IRequest<Result<CategoryContentCreateDTO>>
	{
	}
}
