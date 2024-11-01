using Microsoft.EntityFrameworkCore;
using PhotoGallery.Models.RepositoryDTOs;

namespace PhotoGallery.Data;

public class GalleryDbContext : DbContext
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Album>()
            .HasMany(a => a.Photos)
            .WithMany(p => p.Albums)
            .UsingEntity<Dictionary<string, object>>(
                "AlbumPhoto",
                j => j
                    .HasOne<Photo>()
                    .WithMany()
                    .HasForeignKey("PhotoId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Album>()
                    .WithMany()
                    .HasForeignKey("AlbumId")
                    .OnDelete(DeleteBehavior.Cascade));
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Photo> Photos { get; set; }
}
