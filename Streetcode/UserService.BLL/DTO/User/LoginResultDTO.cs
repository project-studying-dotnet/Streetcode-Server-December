﻿using System;

namespace Streetcode.BLL.DTO.Users
{
    public class LoginResultDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
