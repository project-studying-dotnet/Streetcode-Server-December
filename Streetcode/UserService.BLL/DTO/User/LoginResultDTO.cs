﻿using System;

namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
