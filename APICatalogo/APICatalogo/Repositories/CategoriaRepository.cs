using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters parameters)
        {
            var categorias = await base.GetAsync();

            var categoriasOrdenadas = categorias.OrderBy(c => c.Id).AsQueryable();

            var resultado = PagedList<Categoria>.ToPagedList(categoriasOrdenadas, parameters.PageNumber, parameters.PageSize);

            return resultado;
        }

        public async Task<PagedList<Categoria>> GetCategoriasFiltroAsync(CategoriasFiltroNome parameters)
        {
            var categorias = await base.GetAsync();

            if (!string.IsNullOrEmpty(parameters.Nome))
                categorias = categorias.Where(c => c.Nome.Contains(parameters.Nome)).OrderBy(c => c.Nome);

            return PagedList<Categoria>.ToPagedList(categorias.AsQueryable(), parameters.PageNumber, parameters.PageSize);
        }
    }
}
