using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public IEnumerable<Produto> GetProdutosPorCategoria(int id)
        {
            return base.Get().Where(c => c.CategoriaId == id);
        }

        public IEnumerable<Produto> GetProdutos(ProdutosParameters prodParams)
        {
            return base.Get()
                .OrderBy(p => p.Nome)
                .Skip((prodParams.PageNumber - 1) * prodParams.PageSize)
                .Take(prodParams.PageSize);
        }
    }
}
