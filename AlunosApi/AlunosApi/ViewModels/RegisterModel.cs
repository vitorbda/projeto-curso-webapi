using System.ComponentModel.DataAnnotations;

namespace AlunosApi.ViewModels
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Senhas não conferem")]
        public string ConfirmPassword { get; set; }
    }
}
