using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.FactReorder
{
    public record FactReorderCommand(FactReorderDto FactReorderDto) : IRequest<Result<List<FactDto>>>
    {
    }
}
