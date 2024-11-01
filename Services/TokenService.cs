using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using PhotoGallery.Configurations;
using PhotoGallery.Enums;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhotoGallery.Services;

public sealed class TokenService : ITokenService
{
    private readonly JWTSettings _apiSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(
        IOptions<JWTSettings> apiSettings,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _apiSettings = apiSettings.Value;
        _refreshTokenRepository = refreshTokenRepository;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task<TokenResponseDTO> GenerateTokensAsync(int userId, string role)
    {
        var key = Encoding.ASCII.GetBytes(_apiSettings.SecretKey);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimConstants.ObjectId, userId.ToString()),
            new Claim(ClaimConstants.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_apiSettings.JWTExpireMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.SaveRefreshTokenAsync(
            userId,
            HashRefreshToken(refreshToken),
            DateTime.UtcNow.AddDays(_apiSettings.RefreshExpireDays));

        return new TokenResponseDTO
        {
            AccessToken = tokenHandler.WriteToken(jwtToken),
            RefreshToken = refreshToken
        };
    }

    public ClaimsPrincipal ValidateAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_apiSettings.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ErrorCode> ValidateRefreshTokenAsync(string refreshToken)
    {
        var hashedToken = HashRefreshToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(hashedToken);

        if (storedToken == null)
        {
            return ErrorCode.RefreshTokenWasntFound;
        }

        if (DateTime.UtcNow > storedToken.ExpiryDate)
        {
            return ErrorCode.RefreshTokenExpired;
        }

        return ErrorCode.None;
    }

    public string GenerateNewJwtToken(int userId, string role)
    {
        var key = Encoding.ASCII.GetBytes(_apiSettings.SecretKey);
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimConstants.ObjectId, userId.ToString()),
            new Claim(ClaimConstants.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_apiSettings.JWTExpireMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string HashRefreshToken(string refreshToken)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    public UserClaimsDTO GetUserClaims(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimConstants.ObjectId)?.Value;

        var roleClaim = principal.Claims.FirstOrDefault(x => x.Type == ClaimConstants.Role)?.Value;

        if (int.TryParse(userIdClaim, out int userId))
        {
            return new UserClaimsDTO
            {
                UserId = userId,
                Role = roleClaim
            };
        }

        throw new Exception("User ID or Role claim is not valid.");
    }
}
