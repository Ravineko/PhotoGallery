using Microsoft.Extensions.Options;
using PhotoGallery.Configurations;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Repositories.Interfaces;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Services;

public sealed class PromotionCodeService : IPromotionCodeService
{
    private readonly IPromotionCodeRepository _promotionCodeRepository;
    private readonly ILogger<PromotionCodeService> _logger;
    private readonly CodesExpirySettings _codeSettings;
    private readonly Random _random = new Random();

    public PromotionCodeService(
        IPromotionCodeRepository promotionCodeRepository,
        ILogger<PromotionCodeService> logger,
        IOptions<CodesExpirySettings> codeSettings)
    {
        _promotionCodeRepository = promotionCodeRepository;
        _logger = logger;
        _codeSettings = codeSettings.Value;
    }

    private string GeneratePromotionCode()
    {
        return _random.Next(100000, 999999).ToString();
    }

    public async Task<PromotionServiceDTO> AddPromotionCodeAsync(int userId)
    {
        try
        {
            var promotionCode = GeneratePromotionCode();
            var expiryDate = DateTime.UtcNow.AddMinutes(_codeSettings.PromotionCodeExpiryMinutes);
            var existingCodes = await _promotionCodeRepository.GetExistingCodesAsync(userId);
            if (existingCodes.Any())
            {

                await _promotionCodeRepository.ReplaceOldCodesWithNewAsync(userId, existingCodes, promotionCode, expiryDate);

                return new PromotionServiceDTO
                {
                    IsSuccess = true,
                    PromotionCode = promotionCode,
                };
            }
            else
            {
                await _promotionCodeRepository.AddCodeAsync(userId, promotionCode, expiryDate);

                return new PromotionServiceDTO
                {
                    IsSuccess = true,
                    PromotionCode = promotionCode,
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception while adding Promotion code: {ex.Message}");
            return new PromotionServiceDTO
            {
                IsSuccess = false
            };
        }
    }
}
