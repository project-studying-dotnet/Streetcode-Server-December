using System.Threading.Tasks;
using FluentResults;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Interfaces.User;

public interface IUserService
{
    Task<Result<UserDto>> Registration(RegistrationDto registrationDto);
}