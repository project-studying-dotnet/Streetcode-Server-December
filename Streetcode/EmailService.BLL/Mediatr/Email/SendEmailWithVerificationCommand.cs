using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.Mediatr.Email
{
    public record SendEmailWithVerificationCommand(string Email) : IRequest<Result<Unit>>;
}
