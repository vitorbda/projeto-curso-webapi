﻿
using Microsoft.AspNetCore.Identity;

namespace AlunosApi.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly SignInManager<IdentityUser> _signManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthenticateService(SignInManager<IdentityUser> signManager, UserManager<IdentityUser> userManager)
        {
            _signManager = signManager;
            _userManager = userManager;
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

        public async Task<bool> RegisterUser(string email, string password)
        {
            var appUser = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var result = await _userManager.CreateAsync(appUser);

            if (result.Succeeded) 
                await _signManager.SignInAsync(appUser, isPersistent: false);

            return result.Succeeded;
        }
    }
}
