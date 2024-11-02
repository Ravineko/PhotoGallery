namespace PhotoGallery.Models.ServiceDTOs;

public sealed class UserServiceDTO
{
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string PasswordHash { get; set; }

    public bool TwoFactorEnabled { get; set; }
}
