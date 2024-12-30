using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.UpdateMainPage
{
    public record UpdateStreetcodeMainPageCommand(StreetcodeMainPageUpdateDto StreetcodeMainPage) : IRequest<Result<StreetcodeDto>>
    {
    }
}
