using System.ComponentModel.DataAnnotations;

namespace CategoriasMvc.Models
{
    public class UsuarioViewModel
    {
        [Display(Name = "Nome de usuário")]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
