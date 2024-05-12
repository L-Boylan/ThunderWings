using Microsoft.EntityFrameworkCore;

namespace ThunderWings.Models;

public class BasketContext : DbContext
{
    public BasketContext(DbContextOptions<BasketContext> options ) : base(options)
    {
        
    }

    public DbSet<Basket> Basket { get; set; } = null!;
}