using PhotoGallery.Models.ServiceDTOs;

namespace PhotoGallery.Services.Interfaces;

public interface IPromotionCodeService
{
    Task<PromotionServiceDTO> AddPromotionCodeAsync(int userId);
}
