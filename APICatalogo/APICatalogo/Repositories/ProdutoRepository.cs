using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public IEnumerable<Produto> GetProdutosPorCategoria(int id)
        {
            return Get().Where(c => c.CategoriaId == id);
        }
    }
}
