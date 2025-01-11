using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.Mediatr.Email
{
    public record ConfirmEmailCommand(string Email , string Token) : IRequest<Result<string>>;
}
