using Microsoft.EntityFrameworkCore;

namespace IssueTrackerApi.Data;

public class IssuesDataContext(DbContextOptions<IssuesDataContext> options) : DbContext(options)
{
    public DbSet<CatalogItem> Catalog { get; set; }
    public DbSet<Issue> Issues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("issues-api");
        base.OnModelCreating(modelBuilder);
    }

    public IQueryable<CatalogItem> ActiveCatalogItems
    {
        get
        {
            return Catalog.Where(c => c.Retired == false);
        }
    }
}
