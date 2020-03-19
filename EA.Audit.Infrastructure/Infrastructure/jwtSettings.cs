﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace EA.Audit.Infrastructure
{
    public class JwtSettings
    {
        public JwtSettings(byte[] key, string issuer, string audience)
        {
            Key = key;
            Issuer = issuer;
            Audience = audience;
        }

        public string Issuer { get; }

        public string Audience { get; }

        public byte[] Key { get; }

        public TokenValidationParameters TokenValidationParameters => new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };

        public static JwtSettings FromConfiguration(IConfiguration configuration)
        {
            var issuser = configuration["jwt:issuer"] ?? "defaultissuer";
            var audience = configuration["jwt:audience"] ?? "defaultaudience";
            var base64Key = configuration["jwt:key"];

            byte[] key;
            if (!string.IsNullOrEmpty(base64Key))
            {
                key = Convert.FromBase64String(base64Key);
            }
            else
            {               
                key = new byte[32];
                RandomNumberGenerator.Fill(key);
            }

            return new JwtSettings(key, issuser, audience);
        }
    }
}
