using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.Interfaces.User
{
    public interface IEmailConfirmationService
    {
        Task<Result<string>> ConfirmEmailAsync(string userId, string token);
    }
}
