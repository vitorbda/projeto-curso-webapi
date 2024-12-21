using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using VShop.ProductApi.Models;

namespace VShop.ProductApi.DTOs;

public class ProductDTO
{
    public int Id { get; set; }
    [Required(ErrorMessage = "The name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    [Required(ErrorMessage = "The price is required")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "The description is required")]
    [StringLength(200, MinimumLength = 5)]
    public string Description { get; set; }
    [Required(ErrorMessage = "The stock is required")]
    [Range(1, 9999)]
    public long Stock { get; set; }
    public string ImageUrl { get; set; }
    public string CategoryName { get; }

    [JsonIgnore]
    public Category Category { get; set; }
    public int CategoryId { get; set; }
}
