namespace PhotoGallery.Models.ServiceDTOs;

public sealed class RefreshTokenServiceDTO
{
    public string RefreshToken { get; set; }

    public string AccessToken { get; set; }

    public DateTime ExpiryDate { get; set; }
}
