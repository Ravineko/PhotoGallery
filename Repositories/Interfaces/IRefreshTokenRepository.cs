using PhotoGallery.Models.RepositoryDTOs;

namespace PhotoGallery.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate);

    Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);

    Task<User> GetUserByRefreshTokenAsync(string refreshToken);

    Task<bool> RevokeRefreshTokenAsync(string refreshToken);
}
