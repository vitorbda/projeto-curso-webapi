using CategoriasMvc.Models;
using System.Text;
using System.Text.Json;

namespace CategoriasMvc.Services
{
    public class Autenticacao : IAutenticacao
    {
        const string apiEndpointAutentica = "/api/Auth/login/";
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private TokenViewModel tokenUsuario;
        private readonly HttpClient _httpClient;

        public Autenticacao(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("AutenticaApi");
        }

        public async Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel usuarioVM)
        {
            var usuario = JsonSerializer.Serialize(usuarioVM);
            var content = new StringContent(usuario, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync(apiEndpointAutentica, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    tokenUsuario = await JsonSerializer.DeserializeAsync<TokenViewModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }

            return tokenUsuario;
        }
    }
}
