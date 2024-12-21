using System.Text.Json;
using VShop.Web.Models;
using VShop.Web.Services.Interfaces;

namespace VShop.Web.Services;

public class CategoryService : ICategoryService
{
    private readonly IHttpClientFactory _factory;
    private const string apiEndpoint = "/api/categories/";
    private readonly JsonSerializerOptions _options;

    public CategoryService(IHttpClientFactory factory)
    {
        _factory = factory;
        _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<CategoryViewModel>> GetAllCategories()
    {
        var client = _factory.CreateClient("ProductApi");
        IEnumerable<CategoryViewModel> categories;

        using (var response = await client.GetAsync(apiEndpoint))
        {
            if (!response.IsSuccessStatusCode) return null;

            var apiResponse = await response.Content.ReadAsStreamAsync();
            categories = await JsonSerializer.DeserializeAsync<IEnumerable<CategoryViewModel>>(apiResponse, _options);
        }

        return categories;
    }
}
