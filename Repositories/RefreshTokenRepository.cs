using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.Entities;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly GalleryDbContext _context;

    public RefreshTokenRepository(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.Token == refreshToken && !rt.IsRevoked)
            .FirstOrDefaultAsync();
    }

    public async Task<User> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .Join(_context.RefreshTokens,
                u => u.Id,
                rt => rt.UserId,
                (u, rt) => new { u, rt })
            .Where(joined => joined.rt.Token == refreshToken)
            .Select(joined => joined.u)
            .FirstOrDefaultAsync();
    }

    public async Task SaveRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate)
    {
        var token = new RefreshToken
        {
            UserId = userId,
            Token = refreshToken,
            ExpiryDate = expiryDate
        };

        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .Where(rt => rt.Token == token)
            .FirstOrDefaultAsync();

        if (refreshToken == null) return false;

        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
