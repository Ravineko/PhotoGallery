namespace PhotoGallery.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<bool> IsSuperAdminExistsAsync();
    Task<bool> RoleExistsAsync(int roleId, string roleName);
    Task AddRoleAsync(int roleId, string roleName);
    Task<int?> GetRoleIdAsync(string roleName);
    Task<bool> UpdateUserRolesAsync(int userId, string roleName);
}
