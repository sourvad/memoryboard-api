using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MemoryboardAPI.Services
{
    public class TokenService
    {
        private readonly SymmetricSecurityKey _signingKey;

        public TokenService(IConfiguration configuration)
        {
            string secretKey = configuration["JwtSecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("Secret key for JWT not found in app configuration");
            }

            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string GenerateToken(int userId)
        {
            Claim[] claims =
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            ];

            SigningCredentials credentials = new(_signingKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}