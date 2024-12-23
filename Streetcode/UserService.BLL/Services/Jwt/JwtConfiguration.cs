﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BLL.Services.Jwt
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int AccessTokenLifetime { get; set; }

        public static JwtConfiguration LoadFromConfiguration(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            return new JwtConfiguration
            {
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SecretKey = jwtSettings["SecretKey"],
                AccessTokenLifetime = int.Parse(jwtSettings["AccessTokenLifetime"])
            };
        }
    }
}
