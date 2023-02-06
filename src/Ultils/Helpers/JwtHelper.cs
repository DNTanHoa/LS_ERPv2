using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ultils.Helpers
{
    public class JwtHelper
    {
        public static string GenerateJsonWebToken(string key, string issuer, string audience,
            IEnumerable<Claim> claims,
            int expireMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer, audience,
                claims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
