﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update
{
    public record UpdateRelatedTermCommand(RelatedTermDTO RelatedTerm) : IRequest<Result<RelatedTermDTO>>
    {
    }
}
