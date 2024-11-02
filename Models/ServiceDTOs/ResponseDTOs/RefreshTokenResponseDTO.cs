using PhotoGallery.Enums;

namespace PhotoGallery.Models.ServiceDTOs.ResponseDTOs;

public sealed class RefreshTokenResponseDTO
{
    public string AccessToken { get; set; }

    public ErrorCode? Errorcode { get; set; }
}
