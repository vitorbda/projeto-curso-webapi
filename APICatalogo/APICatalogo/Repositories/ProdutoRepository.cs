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

        public PagedList<Produto> GetProdutos(ProdutosParameters prodParams)
        {
            var produtos = base.Get().OrderBy(p => p.Id).AsQueryable();
            var produtosPaginados = PagedList<Produto>.ToPagedList(produtos, prodParams.PageNumber, prodParams.PageSize);
            return produtosPaginados;
        }
    }
}
