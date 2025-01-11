using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Interfaces.User
{
    public interface IUserPasswordService
    {
        public Task<Result> ResetPassword(PassResetDto passResetDto);
        public Task<Result> ForgotPassword(string email);
        public Task<Result> ChangePassword(PassChangeDto passChangeDto, string username);
    }
}
