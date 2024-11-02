namespace PhotoGallery.Models.RepositoryDTOs;

public sealed class TwoFactorCode
{
    public int Id { get; set; }
    public string Code { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsUsed { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }

}
