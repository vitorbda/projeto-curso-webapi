using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public interface IProdutoRepository
    {
        IQueryable<Produto> GetProdutos();
        Produto GetProduto(int id);
        Produto Create(Produto produto);
        IEnumerable<Produto> Create(IEnumerable<Produto> produtos);
        bool Update(Produto produto);
        bool Delete(int id);        
    }
}
