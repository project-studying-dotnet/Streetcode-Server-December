﻿using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Users
{
    public class TokenRequestDTO
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
