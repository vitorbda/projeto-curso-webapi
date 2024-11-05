using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters parameters)
        {
            var categorias = await base.GetAsync();

            var categoriasOrdenadas = categorias.OrderBy(c => c.Id).AsQueryable();

            //var resultado = PagedList<Categoria>.ToPagedList(categoriasOrdenadas, parameters.PageNumber, parameters.PageSize);

            return await categorias.ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }

        public async Task<IPagedList<Categoria>> GetCategoriasFiltroAsync(CategoriasFiltroNome parameters)
        {
            var categorias = await base.GetAsync();

            if (!string.IsNullOrEmpty(parameters.Nome))
                categorias = categorias.Where(c => c.Nome.Contains(parameters.Nome)).OrderBy(c => c.Nome);

            //PagedList<Categoria>.ToPagedList(categorias.AsQueryable(), parameters.PageNumber, parameters.PageSize);

            return await categorias.ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }
    }
}
