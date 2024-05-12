using Microsoft.EntityFrameworkCore;

namespace ThunderWings.Models;

public class AircraftContext : DbContext
{
    public AircraftContext(DbContextOptions<AircraftContext> options ) : base(options)
    {
        
    }

    public DbSet<Aircraft> Aircrafts { get; set; } = null!;
}