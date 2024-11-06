using APICatalogo.DTOs;
using APICatalogo.Extensions;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(ITokenService tokenService, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IConfiguration config)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName);

            if (!(user is not null && await _userManager.CheckPasswordAsync(user, loginModel.Password)))
                return Unauthorized();

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var token = _tokenService.GenerateAccessToken(authClaims, _config);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.UserName);

            if (userExists is not null)
                return StatusCode(409, "Usuário já existe!");

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return StatusCode(500, "Ocorreu um erro ao criar o usuário");

            return Ok(await Login(new LoginModel { UserName = model.UserName, Password = model.Password }));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(TokenModel model)
        {
            if (model is null)
                return BadRequest();

            var accessToken = model.AcessToken ?? throw new ArgumentNullException(nameof(model));

            var refreshToken = model.RefreshToken ?? throw new ArgumentNullException(nameof(model));

            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken, _config);

            if (principal is null)
                return BadRequest("Ivalid acess token/refresh token");

            var userName = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(userName);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Ivalid acess token/refresh token");

            var newAcessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _config);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAcessToken),
                refreshToken = newRefreshToken
            });
        }

        [HttpPost("revoke/{username}")]
        public async Task<ActionResult> Remove(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null) return BadRequest();

            await user.ClearRefreshToken(_userManager);

            return NoContent();
        }
    }
}
