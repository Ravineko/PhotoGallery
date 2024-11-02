using PhotoGallery.Enums;

namespace PhotoGallery.Models.ServiceDTOs.ResponseDTOs;

public sealed class TwoFactorResponseDTO
{
    public TokenResponseDTO TokenResponseDTO { get; set; }

    public ErrorCode? Errorcode { get; set; }
}
