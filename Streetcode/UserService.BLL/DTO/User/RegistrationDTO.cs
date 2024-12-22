namespace UserService.BLL.DTO.User;

public record RegistrationDTO(
    string userName,
    string fullName,
    string email,
    string password
    );