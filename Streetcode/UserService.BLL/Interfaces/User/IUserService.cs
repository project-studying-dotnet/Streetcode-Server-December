using System.Threading.Tasks;
using FluentResults;
using Streetcode.BLL.DTO.Users;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Interfaces.User;

public interface IUserService
{
    Task<Result<UserDTO>> Registration(RegistrationDTO registrationDto);
}