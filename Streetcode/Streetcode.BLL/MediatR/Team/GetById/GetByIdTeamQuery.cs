﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;

namespace Streetcode.BLL.MediatR.Team.GetById
{
    public record GetByIdTeamQuery(int Id) : IRequest<Result<TeamMemberDto>>;
}
