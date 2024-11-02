namespace PhotoGallery.Models.VMs;

public sealed class VerifyTwoFactorVM
{
    public string Code { get; set; }

    public string Email { get; set; }
}
