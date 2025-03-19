using System.Text;
using System.Text.Json;
using VShop.Web.Models;
using VShop.Web.Services.Interfaces;
using System.Net.Http.Headers;

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

    private void AddAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProducts(string token)
    {
        var client = _factory.CreateClient("ProductApi");
        AddAuthorizationHeader(client, token);

        using (var response = await client.GetAsync(apiEndpoint))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productsVM = await JsonSerializer.DeserializeAsync<IEnumerable<ProductViewModel>>(apiResponse, _options);
        }

        return productsVM;
    }

    public async Task<ProductViewModel> GetProductById(int id, string token)
    {
        var client = _factory.CreateClient("ProductApi");
        AddAuthorizationHeader(client, token);

        using (var response = await client.GetAsync(apiEndpoint + id))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
        }

        return productVM;
    }

    public async Task<ProductViewModel> CreateProduct(ProductViewModel productVM, string token)
    {
        var client = _factory.CreateClient("ProductApi");
        AddAuthorizationHeader(client, token);

        var content = new StringContent(JsonSerializer.Serialize(productVM), Encoding.UTF8, "application/json");

        using (var response = await client.PostAsync(apiEndpoint, content))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productVM = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
        }

        return productVM;
    }

    public async Task<ProductViewModel> UpdateProduct(ProductViewModel productVM, string token)
    {
        var client = _factory.CreateClient("ProductApi");
        AddAuthorizationHeader(client, token);

        var productUpdated = new ProductViewModel();

        using (var response = await client.PutAsJsonAsync(apiEndpoint, productVM))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            productUpdated = await JsonSerializer.DeserializeAsync<ProductViewModel>(apiResponse, _options);
        }

        return productUpdated;
    }

    public async Task<bool> DeleteProduct(int id, string token)
    {
        var client = _factory.CreateClient("ProductApi");
        AddAuthorizationHeader(client, token);

        using (var response = await client.DeleteAsync(apiEndpoint + id))
            return response.IsSuccessStatusCode;
    }
}
