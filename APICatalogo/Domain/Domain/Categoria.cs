using System.Collections.ObjectModel;

namespace Domain.Domain
{
    public class Categoria
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }

        public ICollection<Produto>? Produtos { get; set; } = new Collection<Produto>();
    }
}
