using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create
{
	public record CreateSourceLinkCategoryCommand(SourceLinkCategoryDTO SourceLinkCategory) : IRequest<Result<SourceLinkCategoryDTO>>;
}
