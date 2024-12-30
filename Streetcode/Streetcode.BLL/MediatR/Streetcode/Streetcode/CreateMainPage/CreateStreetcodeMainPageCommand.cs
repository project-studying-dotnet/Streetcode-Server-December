using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.CreateMainPage
{
    public record CreateStreetcodeMainPageCommand(StreetcodeMainPageCreateDto StreetcodeMainPage) : IRequest<Result<StreetcodeDto>>
    {
    }
}
