using Microsoft.EntityFrameworkCore;

namespace AI.DaDataProxy.Entities;

/// <summary>
/// Represents the database context for the project.
/// This class is responsible for configuring the database connection and mapping entity classes to database tables.
/// </summary>
public class DaDataProxyDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the DaDataProxyDbContext class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public DaDataProxyDbContext(DbContextOptions<DaDataProxyDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}