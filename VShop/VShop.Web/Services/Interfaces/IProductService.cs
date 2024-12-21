using VShop.Web.Models;

namespace VShop.Web.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> GetAllProducts();
    Task<ProductViewModel> GetProductById(int id);
    Task<ProductViewModel> CreateProduct(ProductViewModel productVM);
    Task<ProductViewModel> UpdateProduct(ProductViewModel productVM);
    Task<bool> DeleteProduct(int id);
}
