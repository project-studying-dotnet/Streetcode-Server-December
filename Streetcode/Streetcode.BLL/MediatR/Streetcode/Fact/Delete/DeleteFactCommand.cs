using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete
{
	public record DeleteFactCommand(int id) : IRequest<Result<FactDto>>
	{
	}
}
