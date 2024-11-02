using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Models.ServiceDTOs.ResponseDTOs;

namespace PhotoGallery.Services.Interfaces;

public interface IUserService
{
    Task<RegisterResponseDTO> RegisterAsync(UserServiceDTO userServiceDTO);

    Task<LoginResponseDTO> LoginAsync(UserServiceDTO loginRequestDTO);

    Task<TwoFactorResponseDTO> Verify2FA(string code, string userEmail);

    Task<RefreshTokenResponseDTO> RefreshJwtTokenAsync(RefreshTokenServiceDTO refreshTokenServiceDTO);

    Task<PromotionResponseDTO> SendPromotionAsync(int userId);

    Task<ValidatePromotionResponseDTO> ValidatePromotionAsync(string code);

    Task<string> RevokeRefreshTokenAsync(string token);
}
