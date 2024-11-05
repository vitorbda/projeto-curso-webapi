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

        public async Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int id)
        {
            var produtos = await base.GetAsync();

            return produtos.Where(c => c.CategoriaId == id);
        }

        public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters prodParams)
        {
            var produtos = await base.GetAsync();

            var produtosOrdenados = produtos.OrderBy(p => p.Id).AsQueryable();

            return PagedList<Produto>.ToPagedList(produtosOrdenados, prodParams.PageNumber, prodParams.PageSize);
        }

        public async Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos = await base.GetAsync();

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

            var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos.AsQueryable(), produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);

            return produtosFiltrados;
                        
        }
    }
}
