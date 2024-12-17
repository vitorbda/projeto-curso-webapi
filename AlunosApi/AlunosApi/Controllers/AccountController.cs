using AlunosApi.Context;
using AlunosApi.Services;
using AlunosApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AlunosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthenticateService _authenticateService;

        public AccountController(IConfiguration config, IAuthenticateService authenticateService)
        {
            _config = config;
            _authenticateService = authenticateService;
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticateService.RegisterUser(model.Email, model.Password);

            return result  
                ? Ok($"Usuário criado com sucesso")
                : StatusCode(StatusCodes.Status500InternalServerError, "Registro inválido");
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserToken>> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticateService.Authenticate(model.Email, model.Password);

            return result 
                ? Ok(GenerateToken(model))
                : BadRequest("Erro ao realizar login");
        }

        private UserToken GenerateToken(LoginModel model)
        {
            var claims = new[]
            {
                new Claim("email", model.Email),
                new Claim("meuToken", "token"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(20);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds
                );

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
