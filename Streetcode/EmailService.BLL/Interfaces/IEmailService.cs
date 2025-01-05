using EmailService.DAL.Entities;

namespace EmailService.BLL.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Message message);
    }
}
