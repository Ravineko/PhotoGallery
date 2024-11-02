using PhotoGallery.Enums;

namespace PhotoGallery.Models.ServiceDTOs.ResponseDTOs;

public sealed class LoginResponseDTO
{
    public TokenResponseDTO TokenResponseDTO { get; set; }

    public ErrorCode? Errorcode { get; set; }

    public bool IsEmailSent { get; set; }
}
