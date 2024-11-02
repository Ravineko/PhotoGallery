namespace PhotoGallery.Models.VMs;

public sealed class ResetPasswordVM
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}
