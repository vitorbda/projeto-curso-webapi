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

        public PagedList<Produto> GetProdutosFiltroPreco(ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos = base.Get().AsQueryable();

            if (!produtosFiltroPreco.Preco.HasValue || string.IsNullOrEmpty(produtosFiltroPreco.PrecoCriterio))
                return null;

            var criterio = produtosFiltroPreco.PrecoCriterio.ToLower();

            produtos = criterio switch
            {
                "maior" => produtos.Where(p => p.Preco > produtosFiltroPreco.Preco).OrderBy(p => p.Id),
                "menor" => produtos.Where(p => p.Preco < produtosFiltroPreco.Preco).OrderBy(p => p.Id),
                "igual" => produtos.Where(p => p.Preco == produtosFiltroPreco.Preco).OrderBy(p => p.Id),
                _ => null
            };

            var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos, produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);

            return produtosFiltrados;
                        
        }
    }
}
