using PhotoGallery.Models.RepositoryDTOs;

namespace PhotoGallery.Repositories.Interfaces;

public interface IPromotionCodeRepository
{
    Task AddCodeAsync(
       int userId,
       string code,
       DateTime expiryDate);

    Task<PromotionCode> GetCodeInformationAsync(string code);

    Task MarkCodeAsUsedAsync(int id);

    Task<IEnumerable<int>> GetExistingCodesAsync(int userId);

    Task ReplaceOldCodesWithNewAsync(
        int userId,
        IEnumerable<int> codesToMarkAsUsed,
        string newCode,
        DateTime expiryDate);
}
