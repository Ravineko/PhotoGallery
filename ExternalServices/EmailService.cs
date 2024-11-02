using Microsoft.Extensions.Options;
using PhotoGallery.Configurations;
using PhotoGallery.ExternalServices.Interfaces;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PhotoGallery.ExternalServices;

public sealed class EmailService : IEmailService
{
    private readonly IPromotionCodeService _promotionCodeService;
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _emailSettings;
    private readonly Random _random = new Random();
    public const string PromotionLinkFormat = "http://{0}/promote/{1}";
    public const string ResetPasswordLinkFormat = "http://{0}/reset-password/{1}";

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<EmailSettings> emailoptions,
        IPromotionCodeService promotionCodeService)
    {
        _logger = logger;
        _emailSettings = emailoptions.Value;
        _promotionCodeService = promotionCodeService;
    }

    public async Task<bool> SendPromotionEmailAsync(UserServiceDTO user)
    {
        try
        {
            var promotionResponse = await _promotionCodeService.AddPromotionCodeAsync(user.Id);
            if (!promotionResponse.IsSuccess)
            {
                _logger.LogError("Error while receiving promotion code");
                return false;
            }

            var promotionCode = promotionResponse.PromotionCode;
            var client = new SendGridClient(_emailSettings.ApiKey);
            var from = new EmailAddress(_emailSettings.FromEmailAddress, _emailSettings.FromEmailName);
            var to = new EmailAddress(user.Email);
            var promotionLink = string.Format(PromotionLinkFormat, _emailSettings.Domain, promotionCode);

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = _emailSettings.SendGridPromotionTemplateId
            };
            msg.AddTo(to);
            msg.SetTemplateData(new
            {
                userName = user.UserName,
                url = promotionLink
            });

            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation($"SendGrid response status: {response.StatusCode}");

            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogError($"SendGrid error response: {await response.Body.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception while sending email: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> Send2FACodeByEmailAsync(string toEmail, string code)
    {
        var client = new SendGridClient(_emailSettings.ApiKey);
        var from = new EmailAddress(_emailSettings.FromEmailAddress, _emailSettings.FromEmailName);
        var to = new EmailAddress(toEmail);
        var subject = "Your 2FA Code";
        var plainTextContent = $"Your 2FA code is {code}";
        var htmlContent = $"<strong>Your 2FA code is {code}</strong>";
        var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(message);

        return response.IsSuccessStatusCode;
    }

}
