using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update
{
    public record UpdateSourceLinkCategoryCommand(SourceLinkCategoryDTO SourceLinkCategory) : IRequest<Result<SourceLinkCategoryDTO>>;
}