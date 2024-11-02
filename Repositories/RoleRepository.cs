using Microsoft.EntityFrameworkCore;
using PhotoGallery.Data;
using PhotoGallery.Models.RepositoryDTOs;
using PhotoGallery.Repositories.Interfaces;

namespace PhotoGallery.Repositories;

public sealed class RoleRepository : IRoleRepository
{
    private readonly GalleryDbContext _context;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(
        GalleryDbContext context,
        ILogger<RoleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddRoleAsync(int roleId, string roleName)
    {
        var role = new Role { Id = roleId, Name = roleName };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task<int?> GetRoleIdAsync(string roleName)
    {
        var role = await _context.Roles
            .Where(r => r.Name == roleName)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        return role;
    }

    public async Task<bool> IsSuperAdminExistsAsync()
    {
        return await _context.Users
            .AnyAsync(u => u.Role.Name == "SuperAdmin");
    }

    public async Task<bool> RoleExistsAsync(int roleId, string roleName)
    {
        return await _context.Roles
            .AnyAsync(r => r.Id == roleId || r.Name == roleName);
    }

    public async Task<bool> UpdateUserRolesAsync(int userId, string roleName)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role == null)
                {
                    _logger.LogWarning("Role '{RoleName}' not found.", roleName);
                    return false;
                }

                user.Role = role;

                if (roleName == "Admin")
                {
                    user.TwoFactorEnabled = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user roles for userId: {UserId}", userId);
            return false;
        }
        return false;
    }

}
