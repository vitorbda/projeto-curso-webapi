using System.ComponentModel.DataAnnotations;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.DTOs;

public class CategoryDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "The name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; }
}
