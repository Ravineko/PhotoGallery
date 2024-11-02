using PhotoGallery.Models.ServiceDTOs;

namespace PhotoGallery.ExternalServices.Interfaces;

public interface IEmailService
{
    Task<bool> Send2FACodeByEmailAsync(string toEmail, string code);

    Task<bool> SendPromotionEmailAsync(UserServiceDTO user);
}
