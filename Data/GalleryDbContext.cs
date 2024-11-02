using Microsoft.EntityFrameworkCore;
using PhotoGallery.Models.Entities;

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
            .HasOne(a => a.User)
            .WithMany(u => u.Albums)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Photo>()
            .HasOne(p => p.User)
            .WithMany(u => u.Photos)
            .HasForeignKey(p => p.UserId)
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

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TwoFactorCode>()
            .HasOne(tf => tf.User)
            .WithMany(u => u.TwoFactorCodes)
            .HasForeignKey(tf => tf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PromotionCode>()
            .HasOne(tf => tf.User)
            .WithMany(u => u.PromotionCodes)
            .HasForeignKey(tf => tf.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
    public DbSet<PromotionCode> PromotionCodes { get; set; }
}
