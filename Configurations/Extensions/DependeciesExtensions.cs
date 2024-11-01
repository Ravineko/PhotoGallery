using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;

namespace PhotoGallery.Configurations.Extensions;

public static class DependeciesExtensions
{
    public static IServiceCollection AddDependecies(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GalleryDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
