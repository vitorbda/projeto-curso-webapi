using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VShop.Web.Models;

public class ProductViewModel
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    [Range(1, 9999)]
    public decimal Price { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public long Stock { get; set; }
    [Required]
    [DisplayName("Image URL")]
    public string ImageUrl { get; set; }
    public string CategoryName { get; set; }

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    [Display(Name = "Categoria")]
    public int CategoryId { get; set; }
}
