namespace APICatalogo.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }

        ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
