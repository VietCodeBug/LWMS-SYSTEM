using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LWMS.Application.Common.Models;

using LWMS.Application.Common.Interfaces;

namespace LWMS.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _setting;

        public JwtService(JwtSettings settings)
        {
            _setting = settings;
        }

        public string GenerateToken(Guid userId, string fullName, string role, Guid? merchantId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role)
            };

            if (merchantId.HasValue)
            {
                claims.Add(new Claim("MerchantId", merchantId.Value.ToString()));
            }

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