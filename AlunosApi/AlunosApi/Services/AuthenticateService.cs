
using Microsoft.AspNetCore.Identity;

namespace AlunosApi.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly SignInManager<IdentityUser> _signManager;

        public AuthenticateService(SignInManager<IdentityUser> signManager)
        {
            _signManager = signManager;
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            var result = await _signManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);

            return result.Succeeded;
        }

        public async Task Logout()
        {
            await _signManager.SignOutAsync();
        }
    }
}
