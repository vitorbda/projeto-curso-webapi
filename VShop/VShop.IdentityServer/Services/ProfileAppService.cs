using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using VShop.IdentityServer.Data;
using System.Security.Claims;
using Duende.IdentityModel;

namespace VShop.IdentityServer.Services;

public class ProfileAppService : IProfileService
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public ProfileAppService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var id = context.Subject.GetSubjectId();

        var user = await _userManager.FindByIdAsync(id);

        var userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

        var claims = userClaims.Claims.ToList();
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));

        if (_userManager.SupportsQueryableUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));

                if (_roleManager.SupportsRoleClaims)
                {
                    var identityRole = await _roleManager.FindByNameAsync(role);

                    if (identityRole is not null)
                    {
                        claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                    }
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var userId = context.Subject.GetSubjectId();

        var user = await _userManager.FindByIdAsync(userId);

        context.IsActive = user is not null;
    }
}
