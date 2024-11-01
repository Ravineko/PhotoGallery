namespace PhotoGallery.Models.RepositoryDTOs;

public sealed class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public ICollection<Album> Albums { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}
