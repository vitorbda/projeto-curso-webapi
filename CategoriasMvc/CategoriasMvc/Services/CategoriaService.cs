using CategoriasMvc.Models;
using System.Text;
using System.Text.Json;

namespace CategoriasMvc.Services
{
    public class CategoriaService : ICategoriaService
    {
        private const string apiEndpoint = "/api/v1/categoria/";

        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private readonly IHttpClientFactory _clientFactory;

        private CategoriaViewModel categoriaVM;
        private IEnumerable<CategoriaViewModel> categoriasVM;

        private readonly HttpClient _httpClient;

        public CategoriaService(IHttpClientFactory clientFactory)
        {
            //_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _clientFactory = clientFactory;

            _httpClient = _clientFactory.CreateClient("CategoriasApi");
        }

        public async Task<IEnumerable<CategoriaViewModel>> GetCategorias()
        {
            using(var response = await _httpClient.GetAsync(apiEndpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();

                    categoriasVM = await JsonSerializer
                                            .DeserializeAsync<IEnumerable<CategoriaViewModel>>
                                            (apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return categoriasVM;
        }

        public async Task<CategoriaViewModel> GetById(int id)
        {
            using (var response = await _httpClient.GetAsync(apiEndpoint + id))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();

                    categoriaVM = await JsonSerializer
                                            .DeserializeAsync<CategoriaViewModel>
                                            (apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return categoriaVM;
        }

        public async Task<CategoriaViewModel> CriaCategoria(CategoriaViewModel categoriaVM)
        {
            var categoria = JsonSerializer.Serialize(categoriaVM);
            var content = new StringContent(categoria, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(apiEndpoint, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();

                    categoriaVM = await JsonSerializer
                                            .DeserializeAsync<CategoriaViewModel>
                                            (apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return categoriaVM;
        }

        public async Task<bool> AtualizaCategoria(int id, CategoriaViewModel categoriaVM)
        {
            using (var response = await _httpClient.PutAsJsonAsync(apiEndpoint + id, categoriaVM))          
                return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletaCategoria(int id)
        {
            using (var response = await _httpClient.DeleteAsync(apiEndpoint + id))
                return response.IsSuccessStatusCode;
        }
    }
}
