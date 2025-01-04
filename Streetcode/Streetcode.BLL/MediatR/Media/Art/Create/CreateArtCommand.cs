using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using FluentResults;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public record CreateArtCommand(ArtCreateDto newArt) : IRequest<Result<ArtDto>>;
}
