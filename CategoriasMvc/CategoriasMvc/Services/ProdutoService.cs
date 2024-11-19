using CategoriasMvc.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CategoriasMvc.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly HttpClient _httpClient;
        private const string apiEndpoint = "/api/v1/produtos/";
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private ProdutoViewModel produtoVM = null;
        private IEnumerable<ProdutoViewModel> produtosVM = null;

        public ProdutoService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ProdutosApi");
        }

        public async Task<ProdutoViewModel> GetProdutoPorId(int id, string token)
        {
            PutTokenInHeaderAuthorization(token, _httpClient);

            using (var response = await _httpClient.GetAsync(apiEndpoint + id))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    produtoVM = await JsonSerializer.DeserializeAsync<ProdutoViewModel>(apiResponse, _options);
                }
            }

            return produtoVM;
        }

        public async Task<IEnumerable<ProdutoViewModel>> GetProdutos(string token)
        {
            PutTokenInHeaderAuthorization(token, _httpClient);

            using (var response = await _httpClient.GetAsync(apiEndpoint))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    produtosVM = await JsonSerializer.DeserializeAsync<IEnumerable<ProdutoViewModel>>(apiResponse, _options);
                }
            }

            return produtosVM;
        }

        public async Task<ProdutoViewModel> CriaProduto(ProdutoViewModel produtoVM, string token)
        {
            PutTokenInHeaderAuthorization(token, _httpClient);

            var produto = JsonSerializer.Serialize(produtoVM);
            var content = new StringContent(produto, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(apiEndpoint, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    produtoVM = await JsonSerializer.DeserializeAsync<ProdutoViewModel>(apiResponse, _options);
                }
            }

            return produtoVM;
        }

        public async Task<bool> AtualizaProduto(int id, ProdutoViewModel produtoVM, string token)
        {
            PutTokenInHeaderAuthorization(token, _httpClient);

            var produto = JsonSerializer.Serialize(produtoVM);

            using (var response = await _httpClient.PutAsJsonAsync(apiEndpoint + id, produtoVM))            
                return response.IsSuccessStatusCode;            
        }

        public async Task<bool> DeletaProduto(int id, string token)
        {
            PutTokenInHeaderAuthorization(token, _httpClient);

            using (var response = await _httpClient.DeleteAsync(apiEndpoint + id))
                return response.IsSuccessStatusCode;
        }

        private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
