using PhotoGallery.Enums;

namespace PhotoGallery.Models.ServiceDTOs.ResponseDTOs;

public class ValidatePromotionResponseDTO
{
    public bool IsPromotedUser { get; set; }

    public ErrorCode ErrorCode { get; set; }
}
