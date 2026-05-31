using GlossaryService.Models;
using Microsoft.EntityFrameworkCore;


namespace GlossaryService.Common.Setting;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<GlossaryTag> GlossaryTags => Set<GlossaryTag>();
    public DbSet<GlossaryKeyword> GlossaryKeywords => Set<GlossaryKeyword>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // GlossaryTag
        modelBuilder.Entity<GlossaryTag>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Color).HasMaxLength(20);
            e.HasIndex(x => x.DeletedAt);
        });

        // ── GlossaryKeyword ──────────────────────────────────────────
        modelBuilder.Entity<GlossaryKeyword>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Term).HasMaxLength(120).IsRequired();
            e.Property(x => x.Synonyms).HasMaxLength(500).IsRequired();

            e.HasOne(x => x.Tag)
             .WithMany(x => x.Keywords)
             .HasForeignKey(x => x.TagId)
             .OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.DeletedAt);
        });
    }
}