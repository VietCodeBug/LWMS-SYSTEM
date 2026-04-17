using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LWMS.Application.Common.Models;

namespace LWMS.Infrastructure.Services
{
    public class JwtService
    {
        private readonly JwtSettings _setting;

        public JwtService(JwtSettings settings)
        {
            _setting = settings;
        }

        public string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_setting.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _setting.Issuer,
                audience: _setting.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_setting.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}