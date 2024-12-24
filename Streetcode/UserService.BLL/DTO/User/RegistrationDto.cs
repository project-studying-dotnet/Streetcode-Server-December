namespace UserService.BLL.DTO.User;

public class RegistrationDto
{
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public string Password { get; set; }
}