using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.RepositoryDTOs;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly GalleryDbContext _context;

    public UserRepository(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAsync(string userEmail)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == userEmail);
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new User
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                TwoFactorEnabled = u.TwoFactorEnabled
            })
            .FirstOrDefaultAsync();
    }

    public async Task<string> GetUserRoleAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Role.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsUniqueEmailAsync(string email)
    {
        return !await _context.Users
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> IsUniqueUserNameAsync(string username)
    {
        return !await _context.Users
            .AnyAsync(u => u.Username == username);
    }

    public async Task<int> RegisterAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }
}
