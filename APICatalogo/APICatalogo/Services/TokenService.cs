using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APICatalogo.Services
{
    public class TokenService : ITokenService
    {
        public JwtSecurityToken GenerateAccessToke(IEnumerable<Claim> claims, IConfiguration _config)
        {
            var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ?? throw new InvalidOperationException("Invalid secret key");

            var privateKey = Encoding.UTF8.GetBytes(key);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_config.GetValue<double>("JWT:TokenValidityMinutes")),
                Audience = _config["JWT:ValidAudience"],
                Issuer = _config["JWT:ValidIssuer"],
                SigningCredentials = signingCredentials
            };

            return new JwtSecurityTokenHandler().CreateJwtSecurityToken(tokenDescriptor);
        }
        public string GenerateRefreshToken()
        {

        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config)
        {

        }
    }
}