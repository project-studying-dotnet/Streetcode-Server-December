using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using FluentResults;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public record CreateArtCommand(ArtCreateDTO newArt) : IRequest<Result<ArtDTO>>;
}
