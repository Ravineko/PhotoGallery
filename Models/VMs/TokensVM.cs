namespace PhotoGallery.Models.VMs;

public sealed class TokensVM
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public bool TwoFactorNeeded { get; set; }
}
