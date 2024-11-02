namespace PhotoGallery.Models.ServiceDTOs;

public sealed class PasswordResetTokenServiceDTO
{
    public int UserId { get; set; }

    public string Token { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }
}
