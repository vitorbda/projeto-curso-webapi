using System.Text;
using System.Text.Json;
using VShop.Web.Models;
using VShop.Web.Services.Interfaces;

namespace VShop.Web.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _factory;
    private const string apiEndpoint = "/api/products/";
    private readonly JsonSerializerOptions _options;
    private ProductViewModel productVM;
    private IEnumerable<ProductViewModel> productsVM;

    public ProductService(IHttpClientFactory factory)
    {
        _factory = factory;
        _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts()
    {
        var client = _factory.CreateClient("ProductApi");

        using(var response = await client.GetAsync(apiEndpoint))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productsVM = await JsonSerializer.DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);
        }

        return productsVM;
    }

    public async Task<ProductViewModel> GetProductById(int id)
    {
        var client = _factory.CreateClient("ProductApi");

        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
        }

        return productVM;
    }

    public async Task<ProductViewModel> CreateProduct(ProductViewModel productVM)
    {
        var client = _factory.CreateClient("ProductApi");
        var content = new StringContent(JsonSerializer.Serialize(productVM), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
        }

        return productVM;
    }

    public async Task<ProductViewModel> UpdateProduct(ProductViewModel productVM)
    {
        var client = _factory.CreateClient("ProductApi");
        var productUpdated = new ProductViewModel();

        using (var response = await client.PutAsJsonAsync(apiEndpoint, productVM))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productUpdated = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
        }

        return productUpdated;
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var client = _factory.CreateClient("ProductApi");

        using (var response = await client.DeleteAsync(apiEndpoint + id))
            return response.IsSuccessStatusCode;
    }      
}
