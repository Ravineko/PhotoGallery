using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.RepositoryDTOs;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class PromotionCodeRepository : IPromotionCodeRepository
{
    private readonly GalleryDbContext _context;
    private readonly ILogger<PromotionCodeRepository> _logger;

    public PromotionCodeRepository(
        GalleryDbContext context,
        ILogger<PromotionCodeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<int>> GetExistingCodesAsync(int userId)
    {
        return await _context.PromotionCodes
            .Where(pc => pc.UserId == userId && !pc.IsUsed)
            .Select(pc => pc.Id)
            .ToListAsync();
    }

    public async Task ReplaceOldCodesWithNewAsync(
        int userId,
        IEnumerable<int> codesToMarkAsUsed,
        string newCode,
        DateTime expiryDate)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Starting transaction for userId: {UserId}", userId);

            var codesToUpdate = await _context.PromotionCodes
                .Where(pc => codesToMarkAsUsed.Contains(pc.Id) && !pc.IsUsed)
                .ToListAsync();

            foreach (var code in codesToUpdate)
            {
                code.IsUsed = true;
            }

            var newPromotionCode = new PromotionCode
            {
                UserId = userId,
                Code = newCode,
                ExpiryDate = expiryDate,
                IsUsed = false
            };
            _context.PromotionCodes.Add(newPromotionCode);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed for userId: {UserId}", userId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Transaction rolled back for userId: {UserId} due to error", userId);
            throw new Exception("Failed to add promotion code", ex);
        }
    }

    public async Task MarkCodeAsUsedAsync(int id)
    {
        var promotionCode = await _context.PromotionCodes
            .FirstOrDefaultAsync(pc => pc.Id == id && !pc.IsUsed);

        if (promotionCode != null)
        {
            promotionCode.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddCodeAsync(int userId, string code, DateTime expiryDate)
    {
        var promotionCode = new PromotionCode
        {
            UserId = userId,
            Code = code,
            ExpiryDate = expiryDate,
            IsUsed = false
        };

        _context.PromotionCodes.Add(promotionCode);
        await _context.SaveChangesAsync();
    }

    public async Task<PromotionCode> GetCodeInformationAsync(string code)
    {
        return await _context.PromotionCodes
            .Where(pc => pc.Code == code && pc.ExpiryDate > DateTime.UtcNow && !pc.IsUsed)
            .FirstOrDefaultAsync();
    }
}

