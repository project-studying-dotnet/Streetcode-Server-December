using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentResults;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Media.Art.Create
{
    public record CreateArtCommand(ArtCreateDTO newArt) : IRequest<Result<ArtDTO>>;
}
