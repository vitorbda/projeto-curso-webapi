using Microsoft.EntityFrameworkCore;
using APICatalogo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace APICatalogo.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Categoria>? Categoria { get; set; }
        public DbSet<Produto>? Produto { get; set; }
    }
}
