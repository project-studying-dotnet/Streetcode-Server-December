using EmailService.BLL.DTO;
using FluentResults;
using MediatR;

namespace EmailService.BLL.Mediatr.Email
{
    public record SendEmailCommand(EmailDto Email) : IRequest<Result<Unit>>;
}
