using PhotoGallery.Models.RepositoryDTOs;

namespace PhotoGallery.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> IsUniqueUserNameAsync(string username);

    Task<bool> IsUniqueEmailAsync(string email);

    Task<int> RegisterAsync(User user);
    Task<User> GetUserByIdAsync(int userId);

    Task<User> GetUserByEmailAsync(string userEmail);

    Task<string> GetUserRoleAsync(int userId);

}
