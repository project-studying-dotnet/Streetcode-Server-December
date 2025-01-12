using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Media.Audio.Update
{
    public record UpdateAudioCommand(AudioFileBaseUpdateDTO Audio) : IRequest<Result<AudioDto>>;
}
