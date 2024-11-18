using CategoriasMvc.Models;
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

        public CategoriaService(IHttpClientFactory clientFactory)
        {
            //_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _clientFactory = clientFactory;
        }

        public Task<bool> AtualizaCategoria(int id, CategoriaViewModel categoriaVM)
        {
            throw new NotImplementedException();
        }

        public Task<CategoriaViewModel> CriaCategoria(CategoriaViewModel categoriaVM)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletaCategoria(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CategoriaViewModel> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CategoriaViewModel>> GetCategorias()
        {
            throw new NotImplementedException();
        }
    }
}
