﻿using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.DeleteCategoryContent
{
	public record DeleteCategoryContentCommand(int id) : IRequest<Result<StreetcodeCategoryContentDTO>>
	{
	}
}
