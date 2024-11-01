using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Repositories;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Configurations.Extensions;

public static class DependeciesExtensions
{
    public static IServiceCollection AddDependecies(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GalleryDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPhotoRepository, PhotoRepository>();
        /*        services.AddScoped<IUserRepository, UserRepository>();*/
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
