using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent
{
	public record DeleteCategoryContentCommand(int id) : IRequest<Result<Unit>>;
}
