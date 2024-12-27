using System;

namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
