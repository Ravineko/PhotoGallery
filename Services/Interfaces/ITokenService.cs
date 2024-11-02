using PhotoGallery.Enums;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Models.ServiceDTOs.ResponseDTOs;
using System.Security.Claims;

namespace PhotoGallery.Services.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDTO> GenerateTokensAsync(int userId, string role);

    ClaimsPrincipal ValidateAccessToken(string accessToken);

    Task<ErrorCode> ValidateRefreshTokenAsync(string refreshToken);

    string GenerateNewJwtToken(int userId, string role);

    string HashRefreshToken(string refreshToken);

    UserClaimsDTO GetUserClaims(ClaimsPrincipal principal);
}
