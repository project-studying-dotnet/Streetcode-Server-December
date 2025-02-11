﻿using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.MediatR.Media.Audio.GetAll;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Media.Audio.Update;
using Streetcode.DAL.Entities.Media;
using UserService.BLL.Attributes;

namespace Streetcode.WebApi.Controllers.Media
{

    public class AudioController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return HandleResult(await Mediator.Send(new GetAllAudiosQuery()));
        }

        [HttpGet("{streetcodeId:int}")]
        public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
        {
            return HandleResult(await Mediator.Send(new GetAudioByStreetcodeIdQuery(streetcodeId)));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetAudioByIdQuery(id)));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBaseAudio([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new GetBaseAudioQuery(id)));
        }

        [AuthorizeRoles(UserService.DAL.Enums.UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AudioFileBaseCreateDto audio)
        {
            return HandleResult(await Mediator.Send(new CreateAudioCommand(audio)));
        }

        [AuthorizeRoles(UserService.DAL.Enums.UserRole.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            return HandleResult(await Mediator.Send(new DeleteAudioCommand(id)));
        }

        [AuthorizeRoles(UserService.DAL.Enums.UserRole.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Update([FromBody] AudioFileBaseUpdateDTO audio)
        {
            return HandleResult(await Mediator.Send(new UpdateAudioCommand(audio)));
        }
    }
}