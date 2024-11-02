using PhotoGallery.Enums;

namespace PhotoGallery.Models.ServiceDTOs.ResponseDTOs;

public sealed class PromotionResponseDTO
{
    public bool IsEmailSend { get; set; }

    public ErrorCode ErrorCode { get; set; }
}
