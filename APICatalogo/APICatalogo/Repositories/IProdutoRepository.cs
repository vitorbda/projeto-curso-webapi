﻿using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        PagedList<Produto> GetProdutos(ProdutosParameters prodParams);
        IEnumerable<Produto> GetProdutosPorCategoria(int id);
    }
}
