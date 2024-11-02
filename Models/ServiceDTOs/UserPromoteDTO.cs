namespace PhotoGallery.Models.ServiceDTOs;

public sealed class UserPromoteDTO
{
    public int Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool IsSendPromotion { get; set; }

    public DateTime? ExpiryDate { get; set; }
}
