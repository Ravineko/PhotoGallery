using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.ExternalServices;
using PhotoGallery.ExternalServices.Interfaces;
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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPromotionCodeRepository, PromotionCodeRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ITwoFactorAuthCodeRepository, TwoFactorAuthCodeRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();

        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPromotionCodeService, PromotionCodeService>();
        services.AddScoped<IAlbumService, AlbumService>();

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
