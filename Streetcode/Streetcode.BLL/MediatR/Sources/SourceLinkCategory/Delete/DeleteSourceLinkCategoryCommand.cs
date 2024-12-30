using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete
{
	public record DeleteSourceLinkCategoryCommand(int id) : IRequest<Result<Unit>>;
}
