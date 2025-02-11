﻿using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.Delete
{
	public record DeleteStreetcodeCommand(int id) : IRequest<Result<Unit>>;
}
