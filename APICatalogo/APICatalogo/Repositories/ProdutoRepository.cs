using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;
        public ProdutoRepository(AppDbContext context) 
        {
            _context = context;
        }

        public IQueryable<Produto> GetProdutos()
        {
            return _context.Produto;
        }

        public Produto GetProduto(int id)
        {
            var produto = _context.Produto.FirstOrDefault(p => p.Id == id);

            if (produto is null)
                throw new InvalidOperationException("Produto é null");

            return produto;
        }

        public Produto Create(Produto produto)
        {
            if (produto is null)
                throw new InvalidOperationException("Produto é null");

            _context.Add(produto);
            _context.SaveChanges();

            return produto;
        }

        public IEnumerable<Produto> Create(IEnumerable<Produto> produtos)
        {
            if (produtos is null || !produtos.Any())
                throw new InvalidOperationException("Produto é null");

            _context.AddRange(produtos);
            _context.SaveChanges();

            return produtos;
        }

        public bool Update(Produto produto)
        {
            if (produto is null)
                throw new InvalidOperationException("Produto é null");

            if (!_context.Produto.Any(p => p.Id == produto.Id))
                return false;

            _context.Update(produto);
            _context.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var produto = _context.Produto.Find(id);

            if (produto is null)
                return false;

            _context.Remove(produto);
            _context.SaveChanges();

            return true;
        }        
    }
}
