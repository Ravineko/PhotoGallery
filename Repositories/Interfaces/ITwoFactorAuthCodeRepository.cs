using PhotoGallery.Models.Entities;

namespace PhotoGallery.Repositories.Interfaces;

public interface ITwoFactorAuthCodeRepository
{
    Task AddCodeAsync(int userId, string code, DateTime expiryDate);

    Task<TwoFactorCode> GetCodeByIdAsync(int userId);

    Task MarkCodeAsUsedAsync(string code);
}
