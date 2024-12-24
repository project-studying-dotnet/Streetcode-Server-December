using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Interfaces.User
{
    public interface ILoginService
    {
        public Task<Result<string>> Login(LoginDTO loginDto);
    }
}
