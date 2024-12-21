using System.Threading.Tasks;
using UserService.BLL.DTO.User;

namespace UserService.BLL.Interfaces.User;

public interface IUserService
{
    Task<DAL.Entities.Users.User> Registration(RegistrationDTO registrationDto);
}