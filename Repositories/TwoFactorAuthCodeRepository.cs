using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.Entities;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class TwoFactorAuthCodeRepository : ITwoFactorAuthCodeRepository
{
    private readonly GalleryDbContext _context;

    public TwoFactorAuthCodeRepository(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task AddCodeAsync(int userId, string code, DateTime expiryDate)
    {
        var twoFactorCode = new TwoFactorCode
        {
            UserId = userId,
            Code = code,
            ExpiryDate = expiryDate,
            IsUsed = false
        };

        _context.TwoFactorCodes.Add(twoFactorCode);
        await _context.SaveChangesAsync();
    }

    public async Task<TwoFactorCode> GetCodeByIdAsync(int userId)
    {
        return await _context.TwoFactorCodes
            .Where(tfc => tfc.UserId == userId && !tfc.IsUsed && tfc.ExpiryDate > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task MarkCodeAsUsedAsync(string code)
    {
        var twoFactorCode = await _context.TwoFactorCodes
            .Where(tfc => tfc.Code == code && !tfc.IsUsed)
            .FirstOrDefaultAsync();

        if (twoFactorCode != null)
        {
            twoFactorCode.IsUsed = true;
            await _context.SaveChangesAsync();
        }
    }
}
