using AutoMapper;
using Microsoft.Extensions.Options;
using PhotoGallery.Configurations;
using PhotoGallery.Configurations.Constants;
using PhotoGallery.Enums;
using PhotoGallery.ExternalServices.Interfaces;
using PhotoGallery.Models.Entities;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Models.ServiceDTOs.ResponseDTOs;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITwoFactorAuthCodeRepository _twoFactorAuthCodeRepository;
    private readonly IRefreshTokenRepository _tokenRepository;
    private readonly IPromotionCodeRepository _promotionCodeRepository;
    private readonly IEmailService _emailService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly CodesExpirySettings _codeSettings;
    private readonly Random _random = new Random();

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IEmailService emailService,
        ITokenService tokenService,
        ILogger<UserService> logger,
        ITwoFactorAuthCodeRepository twoFactorAuthCodeRepository,
        IRefreshTokenRepository tokenRepository,
        IPromotionCodeRepository promotionCodeRepository,
        IOptions<CodesExpirySettings> codeSettings,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _emailService = emailService;
        _tokenService = tokenService;
        _logger = logger;
        _twoFactorAuthCodeRepository = twoFactorAuthCodeRepository;
        _tokenRepository = tokenRepository;
        _promotionCodeRepository = promotionCodeRepository;
        _codeSettings = codeSettings.Value;
        _mapper = mapper;
    }

    private string GenerateCode()
    {
        return _random.Next(100000, 999999).ToString();
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

    public async Task<RegisterResponseDTO> RegisterAsync(UserServiceDTO userServiceDTO)
    {
        bool ifUserNameUnique = await _userRepository.IsUniqueUserNameAsync(userServiceDTO.UserName);
        if (!ifUserNameUnique)
        {
            return new RegisterResponseDTO()
            {
                Errorcode = ErrorCode.UserNameAlreadyExist
            };
        }

        bool ifUserEmailUnique = await _userRepository.IsUniqueEmailAsync(userServiceDTO.Email);
        if (!ifUserEmailUnique)
        {
            return new RegisterResponseDTO()
            {
                Errorcode = ErrorCode.UserEmailAlreadyExist
            };
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userServiceDTO.Password);

        var user = _mapper.Map<User>(userServiceDTO);
        user.PasswordHash = hashedPassword;
        var roleid = await _roleRepository.GetRoleIdAsync(RoleNames.User);
        user.RoleId = roleid ?? 2;
        var userId = await _userRepository.RegisterAsync(user);

        return new RegisterResponseDTO
        {
            Errorcode = null
        };
    }

    public async Task<LoginResponseDTO> LoginAsync(UserServiceDTO loginRequestDTO)
    {
        var user = await _userRepository.GetUserByEmailAsync(loginRequestDTO.Email);

        if (user == null)
        {
            return new LoginResponseDTO
            {
                Errorcode = ErrorCode.UserNotFound,
            };
        }

        var isPasswordCorrect = VerifyPassword(loginRequestDTO.Password, user.PasswordHash);
        if (!isPasswordCorrect)
        {
            return new LoginResponseDTO
            {
                Errorcode = ErrorCode.WrongPassword
            };
        }

        if (user.TwoFactorEnabled)
        {
            var code = GenerateCode();
            var expiryDate = DateTime.UtcNow.AddMinutes(_codeSettings.TwoFAExpiryMinutes);
            await _twoFactorAuthCodeRepository.AddCodeAsync(user.Id, code, expiryDate);
            var emailSendSuccessfully = await _emailService.Send2FACodeByEmailAsync(loginRequestDTO.Email, code);
            return new LoginResponseDTO
            {
                IsEmailSent = emailSendSuccessfully,
                Errorcode = emailSendSuccessfully ? null : ErrorCode.EmailNotSent
            };
        }

        var role = await _userRepository.GetUserRoleAsync(user.Id);
        var tokens = await _tokenService.GenerateTokensAsync(user.Id, role);
        return new LoginResponseDTO
        {
            TokenResponseDTO = tokens
        };

    }

    public async Task<TwoFactorResponseDTO> Verify2FA(string code, string userEmail)
    {
        var user = await _userRepository.GetUserByEmailAsync(userEmail);
        var twoFactorcode = await _twoFactorAuthCodeRepository.GetCodeByIdAsync(user.Id);
        if (twoFactorcode == null)
        {
            return new TwoFactorResponseDTO
            {
                Errorcode = ErrorCode.TwoFactorCodeWasntFound,
            };
        }

        if (twoFactorcode.IsUsed)
        {
            return new TwoFactorResponseDTO
            {
                Errorcode = ErrorCode.TwoFactorCodeWasUsed,
            };
        }

        if (twoFactorcode.Code == code)
        {
            await _twoFactorAuthCodeRepository.MarkCodeAsUsedAsync(code);
            var role = await _userRepository.GetUserRoleAsync(user.Id);
            return new TwoFactorResponseDTO
            {
                TokenResponseDTO = await _tokenService.GenerateTokensAsync(user.Id, role)
            };
        }

        return new TwoFactorResponseDTO
        {
            Errorcode = ErrorCode.TwoFactorCodeNotMatch
        };
    }

    public async Task<RefreshTokenResponseDTO> RefreshJwtTokenAsync(RefreshTokenServiceDTO refreshTokenServiceDTO)
    {
        var principal = _tokenService.ValidateAccessToken(refreshTokenServiceDTO.AccessToken);
        if (principal == null)
        {
            return new RefreshTokenResponseDTO
            {
                Errorcode = ErrorCode.InvalidAccessToken,
            };
        }

        var refreshTokenValidationResult = await _tokenService.ValidateRefreshTokenAsync(refreshTokenServiceDTO.RefreshToken);
        if (refreshTokenValidationResult != ErrorCode.None)
        {
            return new RefreshTokenResponseDTO
            {
                Errorcode = refreshTokenValidationResult,
            };
        }

        var claims = _tokenService.GetUserClaims(principal);

        var newJwtToken = _tokenService.GenerateNewJwtToken(claims.UserId, claims.Role);

        return new RefreshTokenResponseDTO
        {
            AccessToken = newJwtToken,
        };
    }

    public async Task<PromotionResponseDTO> SendPromotionAsync(int userId)
    {
        var promotionUser = await _userRepository.GetUserByIdAsync(userId);

        if (promotionUser == null)
        {
            return new PromotionResponseDTO
            {
                ErrorCode = ErrorCode.UserNotFound
            };
        }

        var user = _mapper.Map<UserServiceDTO>(promotionUser);

        var sendPromotionEmail = await _emailService.SendPromotionEmailAsync(user);
        if (!sendPromotionEmail)
        {
            return new PromotionResponseDTO
            {
                ErrorCode = ErrorCode.EmailNotSent
            };
        }

        return new PromotionResponseDTO
        {
            IsEmailSend = true,
        };
    }

    public async Task<ValidatePromotionResponseDTO> ValidatePromotionAsync(string code)
    {
        var promotionCode = await _promotionCodeRepository.GetCodeInformationAsync(code);
        if (promotionCode == null)
        {
            return new ValidatePromotionResponseDTO
            {
                ErrorCode = ErrorCode.PromotionCodeNotFound
            };
        }

        var isSuccessPromotion = await _roleRepository.UpdateUserRolesAsync(promotionCode.UserId, RoleNames.Admin);
        if (isSuccessPromotion)
        {
            await _promotionCodeRepository.MarkCodeAsUsedAsync(promotionCode.Id);
            return new ValidatePromotionResponseDTO
            {
                IsPromotedUser = true
            };
        }

        return new ValidatePromotionResponseDTO
        {
            IsPromotedUser = false,
            ErrorCode = ErrorCode.InternalServerError
        };

    }

    public async Task<string> RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = _tokenService.HashRefreshToken(token);
        var success = await _tokenRepository.RevokeRefreshTokenAsync(refreshToken);
        if (!success)
        {
            return "Failed to revoke refresh token.";
        }

        return "Refresh token revoked successfully.";
    }
}
