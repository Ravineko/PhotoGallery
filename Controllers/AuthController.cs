using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotoGallery.Enums;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Models.VMs;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public AuthController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegistrationVM registrationVM)
    {
        var model = _mapper.Map<UserServiceDTO>(registrationVM);
        var result = await _userService.RegisterAsync(model);

        var response = new APIResponse<string>
        {
            IsSuccess = result.Errorcode is null,
            ErrorMessages = result.Errorcode switch
            {
                ErrorCode.UserNameAlreadyExist => new List<string> { "Username or Email already exists" },
                ErrorCode.UserEmailAlreadyExist => new List<string> { "Email already exists" },
                _ => null
            }
        };

        return result.Errorcode switch
        {
            ErrorCode.UserNameAlreadyExist => Conflict(response),
            ErrorCode.UserEmailAlreadyExist => Conflict(response),
            _ => Ok(response)
        };
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(APIResponse<TokensVM>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
    {
        var model = _mapper.Map<UserServiceDTO>(loginVM);
        var loginResponse = await _userService.LoginAsync(model);

        var response = new APIResponse<TokensVM>
        {
            IsSuccess = loginResponse.TokenResponseDTO is not null || loginResponse.IsEmailSent,
            Result = loginResponse.TokenResponseDTO != null
                     ? new TokensVM
                     {
                         AccessToken = loginResponse.TokenResponseDTO.AccessToken,
                         RefreshToken = loginResponse.TokenResponseDTO.RefreshToken,
                     }
                     : (loginResponse.IsEmailSent ? new TokensVM { TwoFactorNeeded = true } : null),
            ErrorMessages = loginResponse.Errorcode switch
            {
                null => null,
                ErrorCode.UserNotFound => new List<string> { "Invalid Credentials" },
                ErrorCode.WrongPassword => new List<string> { "Invalid Credentials" },
                _ => loginResponse.IsEmailSent ? null : new List<string> { "Error sending message for Two factor authentication" }
            }
        };

        return loginResponse.Errorcode switch
        {
            null when loginResponse.TokenResponseDTO is not null => Ok(response),
            null when loginResponse.IsEmailSent => Accepted(response),
            ErrorCode.UserNotFound or ErrorCode.WrongPassword => Unauthorized(response),
            _ => StatusCode(500, response)
        };
    }

    [HttpPost("verify-2fa")]
    [ProducesResponseType(typeof(APIResponse<TokensVM>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorVM verifyTwoFactorVM)
    {
        var twoFactorResponse = await _userService.Verify2FA(verifyTwoFactorVM.Code, verifyTwoFactorVM.Email);

        return twoFactorResponse.Errorcode switch
        {
            ErrorCode.TwoFactorCodeWasntFound => BadRequest(new APIResponse<string>
            {
                ErrorMessages = new List<string> { "Unused two factor code wasn't found" }
            }),
            ErrorCode.TwoFactorCodeWasUsed => BadRequest(new APIResponse<string>
            {
                ErrorMessages = new List<string> { "Two factor code was already used" }
            }),
            ErrorCode.TwoFactorCodeNotMatch => BadRequest(new APIResponse<string>
            {
                ErrorMessages = new List<string> { "Wrong two factor code" }
            }),
            null when twoFactorResponse.TokenResponseDTO is not null => Ok(new APIResponse<TokensVM>
            {
                Result = new TokensVM
                {
                    AccessToken = twoFactorResponse.TokenResponseDTO.AccessToken,
                    RefreshToken = twoFactorResponse.TokenResponseDTO.RefreshToken,
                }
            }),
            _ => StatusCode(500, new APIResponse<string>
            {
                ErrorMessages = new List<string> { "Server error" }
            })
        };
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(APIResponse<TokensVM>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenVM refreshTokenVM)
    {
        var refreshTokenRequest = _mapper.Map<RefreshTokenServiceDTO>(refreshTokenVM);
        var refreshTokenResponse = await _userService.RefreshJwtTokenAsync(refreshTokenRequest);

        return refreshTokenResponse.Errorcode switch
        {
            ErrorCode.RefreshTokenWasntFound => Unauthorized(new APIResponse<string>
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { "Refresh token wasn't found" }
            }),
            ErrorCode.RefreshTokenExpired => Unauthorized(new APIResponse<string>
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { "Refresh token expired, please authorize again" }
            }),
            ErrorCode.UserNotFound => Unauthorized(new APIResponse<string>
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { "User associated with the token wasn't found" }
            }),
            ErrorCode.InvalidAccessToken => Unauthorized(new APIResponse<string>
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { "Access token is invalid or expired" }
            }),
            null when refreshTokenResponse.AccessToken is not null => Ok(new APIResponse<TokensVM>
            {
                Result = new TokensVM
                {
                    AccessToken = refreshTokenResponse.AccessToken,
                    RefreshToken = refreshTokenRequest.RefreshToken
                }
            }),
            _ => StatusCode(500, new APIResponse<string>
            {
                IsSuccess = false,
                ErrorMessages = new List<string> { "Server error" }
            })
        };
    }

    [Authorize(Roles = "SuperAdmin")]
    [HttpPost("promotions")]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendPromotion([FromBody] UserToPromoteVM userToPromoteVM)
    {
        var sendPromotionResponse = await _userService.SendPromotionAsync(userToPromoteVM.UserId);

        var response = sendPromotionResponse.IsEmailSend
            ? new APIResponse<string> { Result = "Successfully sent promotion email" }
            : new APIResponse<string>
            {
                IsSuccess = false,
                ErrorMessages = sendPromotionResponse.ErrorCode switch
                {
                    ErrorCode.EmailNotSent => new List<string> { "Error sending promotion email" },
                    ErrorCode.UserNotFound => new List<string> { "Can't find User to promote" },
                    _ => new List<string> { "Unknown error" }
                }
            };

        return sendPromotionResponse.IsEmailSend
            ? Ok(response)
            : sendPromotionResponse.ErrorCode switch
            {
                ErrorCode.EmailNotSent => StatusCode(500, response),
                ErrorCode.UserNotFound => BadRequest(response),
                _ => BadRequest(response)
            };
    }

    [HttpPost("promotions/validation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PromoteToAdmin([FromBody] PromoteCodeVM promoteCodeVM)
    {
        var promotionResponse = await _userService.ValidatePromotionAsync(promoteCodeVM.Code);
        var response = promotionResponse.IsPromotedUser
            ? new APIResponse<string> { Result = "Successfully promoted user to admin" }
            : new APIResponse<string>
            {
                ErrorMessages = promotionResponse.ErrorCode switch
                {
                    ErrorCode.PromotionCodeNotFound => new List<string> { $"PromotionCode {promoteCodeVM.Code} not found" },
                    ErrorCode.PromotionCodeNotMatch => new List<string> { "Your code doesn't match" },
                    _ => new List<string> { "Unknown error" }
                }
            };

        return promotionResponse.IsPromotedUser
            ? Ok(response)
            : promotionResponse.ErrorCode switch
            {
                ErrorCode.PromotionCodeNotFound => BadRequest(response),
                ErrorCode.PromotionCodeNotMatch => BadRequest(response),
                _ => StatusCode(500, response)
            };
    }

    [HttpPost("revoke-token")]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeTokenVM revokeTokenVM)
    {
        var result = await _userService.RevokeRefreshTokenAsync(revokeTokenVM.RefreshToken);

        if (result.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(500, result);
        }

        return Ok(new APIResponse<string>
        {
            IsSuccess = true
        });
    }
}
