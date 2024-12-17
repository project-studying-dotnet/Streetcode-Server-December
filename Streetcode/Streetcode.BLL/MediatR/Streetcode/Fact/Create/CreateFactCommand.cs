﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create
{
    public record CreateFactCommand(CreateFactDTO Fact) : IRequest<Result<FactDto>>
    {
    }
}