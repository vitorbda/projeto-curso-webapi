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

        public PagedList<Categoria> GetCategorias(CategoriasParameters parameters)
        {
            var categorias = base.Get().OrderBy(c => c.Id).AsQueryable();

            var categoriasOrdenadas = PagedList<Categoria>.ToPagedList(categorias, parameters.PageNumber, parameters.PageSize);

            return categoriasOrdenadas;
        }
    }
}
