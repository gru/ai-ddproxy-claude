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
    /// Gets or sets the DbSet for DaData entities.
    /// This property provides access to query and save instances of DaDataEntity.
    /// </summary>
    public DbSet<DaDataEntity> DaDatas { get; set; }

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DaDataEntity>(entity =>
        {
            entity.ToTable("aggregates");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").HasColumnType("bigint").UseIdentityAlwaysColumn();
            entity.Property(e => e.Name).HasColumnName("name").HasColumnType("text").IsRequired();
        });
    }
}