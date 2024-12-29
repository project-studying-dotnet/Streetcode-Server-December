﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete
{
    public record DeleteRelatedTermCommand(string Word, int TermId) : IRequest<Result<Unit>>;
}
