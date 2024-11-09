using APICatalogo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Extensions
{
    public static class TokenExtensions
    {
        public static bool NotValid(this ClaimsPrincipal claims, SecurityToken securityToken)
        {
            return securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        public static async Task ClearRefreshToken(this ApplicationUser user, UserManager<ApplicationUser> _userManager)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
    }
}
